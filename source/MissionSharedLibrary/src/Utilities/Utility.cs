using MissionSharedLibrary.HotKey;
using System;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
using static TaleWorlds.MountAndBlade.Agent;
using MathF = TaleWorlds.Library.MathF;
using Module = TaleWorlds.MountAndBlade.Module;

namespace MissionSharedLibrary.Utilities
{
    public static class Utility
    {
        public static string ModuleId;

        public static WorldPosition GetOrderPosition(Formation formation)
        {
            return (WorldPosition)(typeof(Formation).GetField("_orderPosition", BindingFlags.Instance | BindingFlags.NonPublic)?
                .GetValue(formation) ?? WorldPosition.Invalid);
        }

        public static bool ShouldDisplayMessage { get; set; } = true;
        public static void DisplayLocalizedText(string id, string variation = null)
        {
            try
            {
                if (!ShouldDisplayMessage)
                    return;
                DisplayMessageImpl(GameTexts.FindText(id, variation).ToString());
            }
            catch
            {
                // ignored
            }
        }
        public static void DisplayLocalizedText(string id, string variation, Color color)
        {
            try
            {
                if (!ShouldDisplayMessage)
                    return;
                DisplayMessageImpl(GameTexts.FindText(id, variation).ToString(), color);
            }
            catch
            {
                // ignored
            }
        }
        public static void DisplayMessage(string msg)
        {
            try
            {
                if (!ShouldDisplayMessage)
                    return;
                DisplayMessageImpl(new TextObject(msg).ToString());
            }
            catch
            {
                // ignored
            }
        }
        public static void DisplayMessage(string msg, Color color)
        {
            try
            {
                if (!ShouldDisplayMessage)
                    return;
                DisplayMessageImpl(new TextObject(msg).ToString(), color);
            }
            catch
            {
                // ignored
            }
        }

        private static void DisplayMessageImpl(string str)
        {
            InformationManager.DisplayMessage(new InformationMessage($"{ModuleId}: " + str));
        }

        private static void DisplayMessageImpl(string str, Color color)
        {
            InformationManager.DisplayMessage(new InformationMessage($"{ModuleId}: " + str, color));
        }

        public static void PrintUsageHint()
        {
            var keyName = GeneralGameKeyCategory.GetKey(GeneralGameKey.OpenMenu).ToSequenceString();
            var hint = Module.CurrentModule.GlobalTextManager.FindText("str_mission_library_open_menu_hint").SetTextVariable("KeyName", keyName).ToString();
            DisplayMessage(hint);
        }

        public static void DisplayMessageForced(string text)
        {
            DisplayMessageImpl(text);
        }

        public static TextObject TextForKey(InputKey key)
        {
            return Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text",
                 new Key(key).ToString().ToLower());
        }

        public static bool IsAgentDead(Agent agent)
        {
            return agent == null || !agent.IsActive();
        }

        public static bool IsPlayerDead()
        {
            return IsAgentDead(Mission.Current.MainAgent);
        }

        public static bool IsTeamValid(Team team)
        {
            return team?.IsValid ?? false;
        }

        public static void SetPlayerAsCommander(bool forced = false)
        {
            var mission = Mission.Current;
            if (!IsTeamValid(mission?.PlayerTeam))
                return;
            mission.PlayerTeam.PlayerOrderController.Owner = mission.MainAgent;
            foreach (var formation in mission.PlayerTeam.FormationsIncludingEmpty)
            {
                if (formation.PlayerOwner != null && formation.PlayerOwner != mission.MainAgent || forced)
                {
                    // set to _playerOwner to avoid changing formation.IsAIControlled
                    typeof(Formation).GetField("_playerOwner", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(formation, mission.MainAgent);
                    //bool isAIControlled = formation.IsAIControlled;
                    //bool isSplittableByAI = formation.IsSplittableByAI;
                    //formation.PlayerOwner = mission.MainAgent;
                    //formation.SetControlledByAI(isAIControlled, isSplittableByAI);
                }
            }
        }

        public static void CancelPlayerAsCommander()
        {
        }

        public static void SetMainAgentFormation(Formation formation)
        {
            // If formation is null, prevent agent be added to detachments during DetachmentManager.TickDetachments
            // Or if agent with no formation is added to a detachment, and later is added to a formation, it will be detached and attached at the same time.
            // See Mission.OnAgentFleeing for example:
            // begin code:
            //
            // if (agent.Formation == null)
            //     return;
            // agent.Formation.Team.DetachmentManager.OnAgentRemoved(agent);
            // agent.Formation = (Formation)null;
            //
            // end code.
            var agent = Mission.Current?.MainAgent;
            if (agent == null)
            {
                return;
            }
            if (formation == null && agent.Formation != null && IsTeamValid(agent.Team))
            {
                agent.Team.DetachmentManager?.OnAgentRemoved(agent);
            }
            agent.Formation = formation;
        }

        public static void SetPlayerFormationClass(FormationClass formationClass)
        {
            if (Mission.Current.IsNavalBattle)
                return;
            if (formationClass < 0 || formationClass >= FormationClass.NumberOfAllFormations)
                return;
            var mission = Mission.Current;
            if (mission.MainAgent != null && IsTeamValid(mission.PlayerTeam))
            {
                var originalFormation = mission.MainAgent.Formation;
                if (originalFormation?.FormationIndex != formationClass)
                {
                    var formation = mission.PlayerTeam.GetFormation(formationClass);
                    if (formation == null)
                        return;
                    if (formation.CountOfUnits == 0)
                    {
                        // If the formation is controlled by AI, the player may be transferred to another formation.
                        if (Mission.Current.PlayerTeam.IsPlayerGeneral && formation.IsAIControlled && formation.FormationIndex < FormationClass.General)
                        {
                            formation.SetControlledByAI(false, formation.IsSplittableByAI);
                        }
                        if (formation.FormationIndex == FormationClass.General)
                        {
                            var generalFormation = Mission.Current.PlayerTeam.GetFormation(FormationClass.General);
                            if (generalFormation.AI.GetBehavior<BehaviorGeneral>() != null)
                            {
                                TacticComponent.SetDefaultBehaviorWeights(generalFormation);
                                generalFormation.AI.SetBehaviorWeight<BehaviorGeneral>(1f);
                                generalFormation.SetControlledByAI(true);
                            }
                        }
                        else if (originalFormation == null || originalFormation.FormationIndex == FormationClass.General)
                        {
                            // fix crash when begin a battle and assign player to an empty formation, then give it an shield wall order.
                            formation.SetMovementOrder(
                                MovementOrder.MovementOrderMove(mission.MainAgent.GetWorldPosition()));
                        }
                        else
                        {
                            // copied from Formation.CopyOrdersFrom
                            CopyOrdersFrom(formation, originalFormation);

                            //formation.SetPositioning(GetOrderPosition(formation), formation.Direction, formation.UnitSpacing);
                        }
                    }

                    SetMainAgentFormation(formation);
                }
            }
        }

        private static void CopyOrdersFrom(Formation formation, Formation target)
        {
            // copied from Formation.CopyOrdersFrom
            formation.SetMovementOrder(target.GetReadonlyMovementOrderReference());
            formation.SetFormOrder(target.FormOrder);
            formation.SetPositioning(unitSpacing: new int?(target.UnitSpacing));
            formation.SetRidingOrder(target.RidingOrder);
            formation.SetFiringOrder(target.FiringOrder);
            formation.SetControlledByAI(target.IsAIControlled || !target.Team.IsPlayerGeneral);
            if (target.AI.Side != FormationAI.BehaviorSide.BehaviorSideNotSet)
                formation.AI.Side = target.AI.Side;
            formation.SetMovementOrder(target.GetReadonlyMovementOrderReference());
            formation.SetTargetFormation(target.TargetFormation);
            formation.SetFacingOrder(target.FacingOrder);
            formation.SetArrangementOrder(target.ArrangementOrder);
        }

        public static bool IsInPlayerParty(Agent agent)
        {
            if (Campaign.Current != null)
            {
                if (agent.Origin is SimpleAgentOrigin simpleAgentOrigin && (simpleAgentOrigin.Party == null || simpleAgentOrigin.Party == Campaign.Current.MainParty?.Party) ||
                    agent.Origin is PartyAgentOrigin partyAgentOrigin && partyAgentOrigin.Party == Campaign.Current.MainParty?.Party ||
                    agent.Origin is PartyGroupAgentOrigin partyGroupAgentOrigin && partyGroupAgentOrigin.Party == Campaign.Current.MainParty?.Party)
                    return true;
            }
            else
            {
                return agent.Team == Mission.Current.PlayerTeam;
            }
            return false;
        }

        public static bool? IsHigherInMemberRoster(Agent lhs, Agent rhs)
        {
            try
            {
                if (Campaign.Current != null)
                {
                    var lhsIsInPlayerParty = IsInPlayerParty(lhs);
                    var rhsIsInPlayerParty = IsInPlayerParty(rhs);
                    if (lhsIsInPlayerParty && !rhsIsInPlayerParty)
                        return true;
                    if (!lhsIsInPlayerParty && rhsIsInPlayerParty)
                        return false;
                    if (!lhsIsInPlayerParty) // && !rhsIsInPlayerParty
                        return null;
                    // lhsIsInPlayerParty && rhsIsInPlayerParty
                    if (Campaign.Current.MainParty?.Party?.MemberRoster == null)
                        return null;
                    var lhsIndex =
                        Campaign.Current.MainParty.Party.MemberRoster.FindIndexOfTroop(lhs.Character as CharacterObject);
                    var rhsIndex =
                        Campaign.Current.MainParty.Party.MemberRoster.FindIndexOfTroop(rhs.Character as CharacterObject);
                    if (lhsIndex == -1 && rhsIndex == -1)
                        return null;
                    return lhsIndex < rhsIndex;
                }
            }
            catch (Exception e)
            {
                DisplayMessage(e.ToString());
            }

            return null;
        }

        public static void PlayerControlAgent(Agent agent)
        {
            if (agent == null)
            {
                return;
            }
            if (Mission.Current?.IsFastForward ?? false)
            {
                Mission.Current.SetFastForwardingFromUI(false);
            }
            // If agent is detached from formation, we need to set HasPlayerControlledTroop
            // Because if IsPlayerTroopInFormation is true, formation's TryRelocatePlayerUnit may result in stack overflow.
            // We need to set HasPlayerControlledTroop before setting agent.Controller to make our patch effective.
            // See Patch_Formation.
            //if (agent.Formation != null && agent.IsDetachedFromFormation)
            //{
            //    SetHasPlayerControlledTroop(agent.Formation, true);
            //}
            agent.Controller = AgentControllerType.Player;
            agent.AIStateFlags = AIStateFlag.None;
            agent.MountAgent?.SetMaximumSpeedLimit(-1f, isMultiplier: false);
            agent.SetMaximumSpeedLimit(-1f, isMultiplier: false);
            if (agent.WalkMode)
            {
                agent.EventControlFlags |= EventControlFlag.Run;
                // required to fix the issue that the agent may still walk after switching to player controller, after deployment.
                agent.EventControlFlags &= ~EventControlFlag.Walk;
            }
            var component = agent.GetComponent<VictoryComponent>();
            if (component != null)
            {
                agent.RemoveComponent(component);
                agent.SetActionChannel(1, ActionIndexCache.act_none, true);
                agent.ClearTargetFrame();
            }
        }

        public static void AIControlMainAgent(bool changeAlarmed, bool alarmed = false)
        {
            var mission = Mission.Current;
            if (mission?.MainAgent == null)
                return;

            try
            {
                mission.GetMissionBehavior<MissionMainAgentController>()?.InteractionComponent.ClearFocus();
                if (mission.MainAgent.Controller != AgentControllerType.AI)
                {
                    var formation = mission.MainAgent.Formation;
                    if (formation != null && mission.MainAgent.IsUsingGameObject && !(mission.MainAgent.CurrentlyUsedGameObject is SpawnedItemEntity))
                    {
                        mission.MainAgent.HandleStopUsingAction();
                    }

                    // if (mission.MainAgent.HumanAIComponent != null)
                    // {
                    //     // HumanAIComponent registered the following action in constructor, but didn't unregister it.
                    //     // TODO: Need to check the official code whether this fix affects other behaviors.
                    //     if (mission.MainAgent.OnAgentWieldedItemChange != null)
                    //         mission.MainAgent.OnAgentWieldedItemChange -=
                    //             mission.MainAgent.HumanAIComponent.DisablePickUpForAgentIfNeeded;
                    //     if (mission.MainAgent.OnAgentMountedStateChanged != null)
                    //         mission.MainAgent.OnAgentMountedStateChanged -=
                    //             mission.MainAgent.HumanAIComponent.DisablePickUpForAgentIfNeeded;
                    //     mission.MainAgent.RemoveComponent(mission.MainAgent.HumanAIComponent);
                    // }

                    //mission.MainAgent.Formation = null;
                    mission.MainAgent.Controller = AgentControllerType.AI;

                    // This is to resolve the issue that player cannot ride horse afte switching to AI and switching back.
                    // no longer required.
                    //if (mission.MainAgent.Character?.Equipment[EquipmentIndex.Horse].Item != null || (mission.MainAgent.Character?.IsMounted ?? false))
                    //{
                    //    mission.MainAgent.SetAgentFlags(mission.MainAgent.GetAgentFlags() | AgentFlag.CanRide);
                    //}
                    //else
                    //{
                    //    mission.MainAgent.SetAgentFlags(mission.MainAgent.GetAgentFlags() & ~AgentFlag.CanRide);
                    //}
                    // Note that the formation may be already set by SwitchFreeCameraLogic
                    //if (mission.MainAgent.Formation == null)
                    //{
                    //    SetMainAgentFormation(formation);
                    //}
                    // the Initialize method need to be called manually.
                    try
                    {
                        mission.MainAgent.CommonAIComponent?.Initialize();
                        mission.MainAgent.HumanAIComponent?.Initialize();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        DisplayMessage(e.ToString());
                    }

                    // TODO: Need to check whether the following is needed.
                    //mission.MainAgent.ResetEnemyCaches();
                    //mission.MainAgent.InvalidateTargetAgent();
                    //mission.MainAgent.InvalidateAIWeaponSelections();
                    if (mission.MainAgent.Formation != null)
                    {
                        mission.MainAgent.SetRidingOrder(mission.MainAgent.Formation.RidingOrder.OrderEnum);
                    }
                    if (changeAlarmed)
                    {
                        if (alarmed)
                        {
                            if ((mission.MainAgent.AIStateFlags & Agent.AIStateFlag.Alarmed) == Agent.AIStateFlag.None)
                                SetMainAgentAlarmed(true);
                        }
                        else
                        {
                            SetMainAgentAlarmed(false);
                        }
                    }
                    if (mission.MainAgent.IsPaused)
                    {
                        mission.MainAgent.SetIsAIPaused(false);
                    }

                    mission.MainAgent.Formation?.GetReadonlyMovementOrderReference()
                        .OnUnitJoinOrLeave(mission.MainAgent.Formation, mission.MainAgent, true);
                }
            }
            catch (Exception e)
            {
                DisplayMessage(e.ToString());
            }
        }

        public static void SetMainAgentAlarmed(bool alarmed)
        {
            Mission.Current.MainAgent?.SetWatchState(alarmed
                ? Agent.WatchState.Alarmed
                : Agent.WatchState.Patrolling);
        }

        public static bool IsEnemy(Agent agent)
        {
            return Mission.Current.MainAgent?.IsEnemyOf(agent) ??
                   Mission.Current.PlayerTeam?.IsEnemyOf(agent.Team) ?? false;
        }

        public static bool IsEnemy(Formation formation)
        {
            return Mission.Current.PlayerTeam?.IsEnemyOf(formation.Team) ?? false;
        }

        private static readonly FieldInfo CameraAddedElevation =
            typeof(MissionScreen).GetField("_cameraAddedElevation", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo CameraTargetAddedHeight =
            typeof(MissionScreen).GetField("_cameraTargetAddedHeight", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo CameraAddSpecialMovement =
            typeof(MissionScreen).GetField("_cameraAddSpecialMovement", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo CameraApplySpecialMovementsInstantly =
            typeof(MissionScreen).GetField("_cameraApplySpecialMovementsInstantly", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo SetLastFollowedAgent =
            typeof(MissionScreen).GetProperty("LastFollowedAgent", BindingFlags.Instance | BindingFlags.Public)?.GetSetMethod(true);

        private static readonly FieldInfo CameraSpecialCurrentAddedElevation =
            typeof(MissionScreen).GetField("_cameraSpecialCurrentAddedElevation", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo CameraSpecialCurrentAddedBearing =
            typeof(MissionScreen).GetField("_cameraSpecialCurrentAddedBearing", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo CameraSpecialCurrentPositionToAdd =
            typeof(MissionScreen).GetField("_cameraSpecialCurrentPositionToAdd", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo CameraSpecialCurrentDistanceToAdd =
            typeof(MissionScreen).GetField("_cameraSpecialCurrentDistanceToAdd", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo SetCameraElevation =
            typeof(MissionScreen).GetProperty("CameraElevation", BindingFlags.Instance | BindingFlags.Public)
                ?.GetSetMethod(true);

        private static readonly MethodInfo SetCameraBearing =
            typeof(MissionScreen).GetProperty("CameraBearing", BindingFlags.Instance | BindingFlags.Public)
                ?.GetSetMethod(true);

        private static readonly FieldInfo IsPlayerAgentAdded =
            typeof(MissionScreen).GetField("_isPlayerAgentAdded", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool ShouldSmoothMoveToAgent = true;

        public static bool BeforeSetMainAgent(Agent agent)
        {
            if (ShouldSmoothMoveToAgent && GetMissionScreen().LastFollowedAgent != agent)
            {
                ShouldSmoothMoveToAgent = false;
                return true;
            }

            return false;
        }

        public static void AfterSetMainAgent(bool shouldSmoothMoveToAgent, MissionScreen missionScreen, bool rotateCamera = true)
        {
            if (shouldSmoothMoveToAgent)
            {
                ShouldSmoothMoveToAgent = true;
                SmoothMoveToAgent(missionScreen, false, rotateCamera);
            }
            else
            {
                SetIsPlayerAgentAdded(missionScreen, false);
            }
        }

        public static void SmoothMoveToAgent(MissionScreen missionScreen, bool forceMove = false, bool changeCameraRotation = true)
        {
            try
            {
                var spectatingData = missionScreen.GetSpectatingData(missionScreen.CombatCamera.Position);
                if (spectatingData.AgentToFollow != null)
                {
                    CameraAddSpecialMovement?.SetValue(missionScreen, true);
                    CameraApplySpecialMovementsInstantly?.SetValue(missionScreen, false);
                    if (missionScreen.LastFollowedAgent != spectatingData.AgentToFollow || forceMove)
                    {
                        float virtualElevation = changeCameraRotation ? 0 : missionScreen.CameraElevation - (float)CameraAddedElevation.GetValue(missionScreen);
                        float virtualBearing = changeCameraRotation ? spectatingData.AgentToFollow.LookDirectionAsAngle : missionScreen.CameraBearing;
                        var targetFrame = GetCameraFrameWhenLockedToAgent(missionScreen, spectatingData.AgentToFollow, spectatingData.CameraType, virtualElevation, virtualBearing);
                        SmoothMoveToPositionAndDirection(missionScreen, targetFrame.origin, 0,
                            spectatingData.AgentToFollow.LookDirectionAsAngle, changeCameraRotation, changeCameraRotation);
                    }

                    SetLastFollowedAgent.Invoke(missionScreen, new object[] { spectatingData.AgentToFollow });
                }
                // Avoid MissionScreen._cameraSpecialCurrentAddedBearing reset to 0.
                SetIsPlayerAgentAdded(missionScreen, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void SmoothMoveToPositionAndDirection(MissionScreen missionScreen, Vec3 position, float elevation,
            float bearing, bool changeElevation, bool changeBearing)
        {
            try
            {
                CameraAddSpecialMovement?.SetValue(missionScreen, true);
                CameraApplySpecialMovementsInstantly?.SetValue(missionScreen, false);
                CameraSpecialCurrentPositionToAdd?.SetValue(missionScreen,
                    missionScreen.CombatCamera.Position - position);
                var cameraAddedElevation = (float)CameraAddedElevation.GetValue(missionScreen);
                if (changeElevation)
                {
                    CameraSpecialCurrentAddedElevation?.SetValue(missionScreen, missionScreen.CameraElevation - cameraAddedElevation - elevation);
                    SetCameraElevation?.Invoke(missionScreen, new object[] { elevation });
                }
                else
                {
                    //CameraSpecialCurrentAddedElevation?.SetValue(missionScreen, 0);
                    SetCameraElevation?.Invoke(missionScreen, new object[] { missionScreen.CameraElevation - cameraAddedElevation });
                }
                

                if (changeBearing)
                {
                    CameraSpecialCurrentAddedBearing?.SetValue(missionScreen,
                        MBMath.WrapAngle(missionScreen.CameraBearing - bearing));
                    SetCameraBearing?.Invoke(missionScreen,
                        new object[] { bearing });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static MatrixFrame GetCameraFrameWhenLockedToAgent(MissionScreen missionScreen, Agent agentToFollow, SpectatorCameraTypes cameraType, float virtualCameraElevation, float virtualCameraBearing)
        {
            MatrixFrame result = MatrixFrame.Identity;
            //float cameraBaseDistance = 0.6f;
            float agentScale = agentToFollow.AgentScale;
            //float cameraTargetAddedElevation;
            //if (agentToFollow.HasMount)
            //{
            //    cameraBaseDistance += 0.1f;
            //}
            //if ((!missionScreen.IsViewingCharacter() && !(cameraType == SpectatorCameraTypes.LockToTeamMembersView && agentToFollow != missionScreen.Mission.MainAgent)) || missionScreen.IsPhotoModeEnabled)
            //{
            //    cameraBaseDistance += 0.5f;
            //}
            result.rotation.RotateAboutSide(1.57079637f);
            result.rotation.RotateAboutForward(virtualCameraBearing);
            result.rotation.RotateAboutSide(virtualCameraElevation);
            if (missionScreen.IsPhotoModeEnabled)
            {
                float a = -missionScreen.Mission.Scene.GetPhotoModeRoll();
                result.rotation.RotateAboutUp(a);
            }
            MatrixFrame matrixFrame = result;
            //float num18 = cameraBaseDistance + (float)CameraSpecialCurrentDistanceToAdd.GetValue(missionScreen);
            //float num22 = MathF.Max(num18 + Mission.CameraAddedDistance, 0.48f) * agentScale;
            //if (missionScreen.Mission.Mode != MissionMode.Conversation && missionScreen.Mission.Mode != MissionMode.Barter && agentToFollow != null && agentToFollow.IsActive() && BannerlordConfig.EnableVerticalAimCorrection)
            //{
            //    MissionWeapon wieldedWeapon = agentToFollow.WieldedWeapon;
            //    WeaponComponentData currentUsageItem = wieldedWeapon.CurrentUsageItem;
            //    if (currentUsageItem != null && currentUsageItem.IsRangedWeapon)
            //    {
            //        MatrixFrame frame = missionScreen.CombatCamera.Frame;
            //        frame.rotation.RotateAboutSide(-(float)CameraAddedElevation.GetValue(missionScreen));
            //        float num23;
            //        if (agentToFollow.HasMount)
            //        {
            //            Agent mountAgent = agentToFollow.MountAgent;
            //            Monster monster = mountAgent.Monster;
            //            num23 = (float)(((double)monster.RiderCameraHeightAdder + (double)monster.BodyCapsulePoint1.z + (double)monster.BodyCapsuleRadius) * (double)mountAgent.AgentScale + (double)agentToFollow.Monster.CrouchEyeHeight * (double)agentScale);
            //        }
            //        else
            //            num23 = agentToFollow.Monster.StandingEyeHeight * agentScale;
            //        if (currentUsageItem.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.UseHandAsThrowBase))
            //            num23 *= 1.25f;
            //        float num24;
            //        {
            //            double z = (double)frame.origin.z;
            //            double num27 = -(double)frame.rotation.u.z;
            //            Vec2 asVec2_7 = frame.origin.AsVec2;

            //            Vec3 closestPoint1 = agentToFollow.Position;
            //            Vec2 asVec2_8 = closestPoint1.AsVec2;
            //            double length = (double)(asVec2_7 - asVec2_8).Length;
            //            double num28 = num27 * length;
            //            num24 = (float)(z + num28 - ((double)agentToFollow.Position.z + (double)num23));
            //        }
            //        if ((double)num24 > 0.0)
            //        {
            //            double num29 = (double)MathF.Sqrt(19.6f * num24);
            //            wieldedWeapon = agentToFollow.WieldedWeapon;
            //            double speedForCurrentUsage = (double)wieldedWeapon.GetModifiedMissileSpeedForCurrentUsage();
            //            cameraTargetAddedElevation = MathF.Max(-0.15f, (float)-(double)MathF.Asin(MathF.Min(1f, (float)(num29 / speedForCurrentUsage))));
            //        }
            //        else
            //            cameraTargetAddedElevation = 0.0f;
            //    }
            //    else
            //        cameraTargetAddedElevation = TaleWorlds.Core.ManagedParameters.Instance.GetManagedParameter(TaleWorlds.Core.ManagedParametersEnum.MeleeAddedElevationForCrosshair);
            //}
            if (!missionScreen.IsPhotoModeEnabled)
                result.rotation.RotateAboutSide((float)CameraAddedElevation.GetValue(missionScreen));
            bool flag6 = agentToFollow.AgentVisuals != null && (uint)agentToFollow.AgentVisuals.GetSkeleton().GetCurrentRagdollState() != 0;
            var agentVisualPosition = agentToFollow.VisualPosition;
            var cameraTarget = flag6 ? agentToFollow.AgentVisuals.GetFrame().origin : agentVisualPosition;
            if (agentToFollow.HasMount)
            {
                var vec2_6 = agentToFollow.MountAgent.GetMovementDirection() * agentToFollow.MountAgent.Monster.RiderBodyCapsuleForwardAdder;
                cameraTarget += vec2_6.ToVec3();
            }
            
            cameraTarget.z += (float)CameraTargetAddedHeight.GetValue(missionScreen);
            if (missionScreen.Mission.Mode != MissionMode.Conversation && missionScreen.Mission.Mode != MissionMode.Barter)
                cameraTarget += matrixFrame.rotation.f * agentScale * (0.7f * MathF.Pow(MathF.Cos((float)(1.0 / ((missionScreen.CameraResultDistanceToTarget / (double)agentScale - 0.20000000298023224) * 30.0 + 20.0))), 3500f));
            result.origin = cameraTarget + matrixFrame.rotation.u * missionScreen.CameraResultDistanceToTarget;
            return result;
        }

        public static Vec3 GetCameraTargetPositionWhenLockedToAgent(MissionScreen missionScreen,
            Agent agentToFollow)
        {
            bool flag5 = agentToFollow.AgentVisuals != null && (uint)agentToFollow.AgentVisuals.GetSkeleton().GetCurrentRagdollState() > 0;
            var agentVisualPosition = agentToFollow.VisualPosition;
            var cameraTarget = flag5 ? agentToFollow.AgentVisuals.GetFrame().origin : agentVisualPosition;
            if (agentToFollow.MountAgent != null)
            {
                var vec3_4 = agentToFollow.MountAgent.GetMovementDirection() * agentToFollow.MountAgent.Monster.RiderBodyCapsuleForwardAdder;
                cameraTarget += vec3_4.ToVec3();
            }
            cameraTarget.z += (float)CameraTargetAddedHeight.GetValue(missionScreen);
            return cameraTarget;
        }

        public static void SetIsPlayerAgentAdded(MissionScreen missionScreen, bool value)
        {
            IsPlayerAgentAdded?.SetValue(missionScreen, value);
            if (value)
                CameraSpecialCurrentPositionToAdd?.SetValue(missionScreen, Vec3.Zero);
        }

        private static readonly PropertyInfo HasPlayerControlledTroop =
            typeof(Formation).GetProperty(nameof(HasPlayerControlledTroop), BindingFlags.Instance | BindingFlags.Public);
        private static readonly PropertyInfo IsPlayerTroopInFormation =
            typeof(Formation).GetProperty(nameof(IsPlayerTroopInFormation), BindingFlags.Instance | BindingFlags.Public);

        private static readonly MethodInfo SetHasPlayerControlledTroopMethod = HasPlayerControlledTroop?.GetSetMethod(true);
        private static readonly MethodInfo SetIsPlayerTroopInFormationMethod = IsPlayerTroopInFormation?.GetSetMethod(true);

        public static void SetIsPlayerTroopInFormation(Formation formation, bool hasPlayer)
        {
            try
            {
                if (formation == null)
                    return;
                SetIsPlayerTroopInFormationMethod?.Invoke(formation, new object[] { hasPlayer });
                formation.OnUnitAddedOrRemoved();
            }
            catch (Exception e)
            {
                DisplayMessage(e.ToString());
            }
        }
        public static void SetHasPlayerControlledTroop(Formation formation, bool hasPlayer)
        {
            try
            {
                if (formation == null)
                    return;
                SetHasPlayerControlledTroopMethod?.Invoke(formation, new object[] { hasPlayer });
                formation.OnUnitAddedOrRemoved();
            }
            catch (Exception e)
            {
                DisplayMessage(e.ToString());
            }
        }

        public static void Reset(this GameKey gameKey)
        {

            Key controllerKey = gameKey.ControllerKey;
            if (controllerKey != null)
            {
                Key defaultControllerKey = gameKey.DefaultControllerKey;
                controllerKey.ChangeKey(defaultControllerKey?.InputKey ?? InputKey.Invalid);
            }
            Key keyboardKey = gameKey.KeyboardKey;
            if (keyboardKey != null)
            {
                Key defaultKeyboardKey = gameKey.DefaultKeyboardKey;
                keyboardKey.ChangeKey(defaultKeyboardKey?.InputKey ?? InputKey.Invalid);
            }
        }

        //For debug purpose

        public static bool CheckAllFormationArrangementIntegrity()
        {
            if (Mission.Current == null || Mission.Current.PlayerTeam == null)
                return true;
            bool result = true;
            foreach (var team in Mission.Current.Teams)
            {
                if (!CheckAllFormationInTeamArrangementIntegrity(team))
                {
                    result = false;
                }
            }
            return result;
        }
        public static bool CheckAllFormationInTeamArrangementIntegrity(Team team)
        {
            bool result = true;
            foreach (var formation in team.FormationsIncludingEmpty)
            {
                if (!CheckFormationArrangementIntegrity(formation))
                {
                    result = false;
                }
            }
            return result;
        }

        public static bool CheckFormationArrangementIntegrity(Formation formation)
        {
            var lineFormation = formation.Arrangement as LineFormation;
            if (lineFormation == null)
                return true;
            MBList2D<IFormationUnit> units2D = (MBList2D<IFormationUnit>)typeof(LineFormation).GetField("_units2D", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(lineFormation);
            if (units2D == null)
                return true;
            for (int i = 0; i < units2D.Count1; i++)
            {
                for (int j = 0; j < units2D.Count2; j++)
                {
                    IFormationUnit unit = units2D[i, j];
                    if (unit != null &&(unit.FormationFileIndex != i || unit.FormationRankIndex != j))
                    {
                        DisplayMessage($"Formation integrity check failed: Agent {((Agent)unit).Name} is in formation {((Agent)unit).Formation?.FormationIndex} has wrong file/rank index");
                        return false;
                    }
                }
            }

            return true;
        }

        public static MissionOrderVM GetMissionOrderVM(Mission mission)
        {
            var ui = mission.GetMissionBehavior<MissionGauntletSingleplayerOrderUIHandler>();
            if (ui != null)
            {
                var dataSource = typeof(MissionGauntletSingleplayerOrderUIHandler).GetField("_dataSource", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(ui) as MissionOrderVM;
                return dataSource;
            }
            return null;
        }

        public static OrderItemVM FindOrderWithId(MissionOrderVM missionOrderVM, string orderId)
        {
            for (int index1 = 0; index1 < missionOrderVM.OrderSets.Count; ++index1)
            {
                OrderSetVM orderSet = missionOrderVM.OrderSets[index1];
                for (int index2 = 0; index2 < orderSet.Orders.Count; ++index2)
                {
                    OrderItemVM order = orderSet.Orders[index2];
                    if (order.OrderIconId == orderId)
                        return order;
                }
            }
            return (OrderItemVM)null;
        }

        public static MissionScreen GetMissionScreen()
        {
            return MissionState.Current.GetListenerOfType<MissionScreen>();
        }

        public static MissionBehavior GetMissionBehaviorOfType(Mission mission, Type type)
        {
            for (int index = 0; index < mission.MissionBehaviors.Count; ++index)
            {
                if (type.IsAssignableFrom(mission.MissionBehaviors[index].GetType()))
                    return mission.MissionBehaviors[index];
            }
            return default;
        }

        public static bool IsModuleInstalled(string moduleId)
        {
            try
            {
                // some module may be not loaded, causes info be null.
                return TaleWorlds.Engine.Utilities.GetModulesNames().Select(ModuleHelper.GetModuleInfo).FirstOrDefault(info =>
                    info?.Id == moduleId) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DisplayMessage(e.ToString());
                MBDebug.Print(e.ToString());
                return false;
            }
        }

        public static bool IsHideoutBattle()
        {
            return MissionState.Current?.MissionName == "HideoutBattle";
        }

        public static bool IsHideoutAmbush()
        {
            return MissionState.Current?.MissionName == "HideoutAmbush";
        }
    }
}
