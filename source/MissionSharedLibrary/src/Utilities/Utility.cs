﻿using MissionSharedLibrary.HotKey.Category;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
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
            var keyName = GeneralGameKeyCategories.GetKey(GeneralGameKey.OpenMenu).ToSequenceString();
            var hint = Module.CurrentModule.GlobalTextManager.FindText("str_rts_camera_open_menu_hint").SetTextVariable("KeyName", keyName).ToString();
            DisplayMessageForced(hint);
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
                if (formation.PlayerOwner != null || forced)
                {
                    bool isAIControlled = formation.IsAIControlled;
                    bool isSplittableByAI = formation.IsSplittableByAI;
                    formation.PlayerOwner = mission.MainAgent;
                    formation.SetControlledByAI(isAIControlled, isSplittableByAI);
                }
            }
        }

        public static void CancelPlayerAsCommander()
        {
        }

        public static void SetMainAgentFormation(Agent agent, Formation formation)
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
            if (formation == null && agent.Formation != null && IsTeamValid(agent.Team))
            {
                agent.Team.DetachmentManager?.OnAgentRemoved(agent);
            }
            //SetHasPlayerControlledTroop(mission.MainAgent.Formation, false);
            //SetIsPlayerTroopInFormation(mission.MainAgent.Formation, false);
            // setting HasPlayerControlledTroop after setting formation is too late:
            // see Patch_Formation.Prefix_Arrangement_OnShapeChanged, we use HasPlayerControlledTroop and IsPlayerTroopInFormation to break the infinite recusion,
            // which happens before Formation set HasPlayerControlledTroop to true.
            // so we have to set HasPlayerControlledTroop and IsPlayerTroopInFormation before setting formation.
            SetHasPlayerControlledTroop(formation, agent.IsPlayerControlled);
            if (agent.IsPlayerTroop)
            {
                SetIsPlayerTroopInFormation(formation, true);
            }
            agent.Formation = formation;
            //SetHasPlayerControlledTroop(mission.MainAgent.Formation, mission.MainAgent.IsPlayerControlled);
            //SetIsPlayerTroopInFormation(mission.MainAgent.Formation, mission.MainAgent.IsPlayerTroop);
        }

        public static void SetPlayerFormationClass(FormationClass formationClass)
        {
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
                        if (originalFormation == null)
                        {
                            // fix crash when begin a battle and assign player to an empty formation, then give it an shield wall order.
                            formation.SetMovementOrder(
                                MovementOrder.MovementOrderMove(mission.MainAgent.GetWorldPosition()));
                        }
                        else
                        {
                            // copied from Formation.CopyOrdersFrom
                            formation.SetMovementOrder(originalFormation.GetReadonlyMovementOrderReference());
                            formation.FormOrder = originalFormation.FormOrder;
                            formation.SetPositioning(unitSpacing: originalFormation.UnitSpacing);
                            formation.RidingOrder = originalFormation.RidingOrder;
                            formation.FiringOrder = originalFormation.FiringOrder;
                            formation.SetControlledByAI(originalFormation.IsAIControlled || !originalFormation.Team.IsPlayerGeneral, originalFormation.IsSplittableByAI);
                            if (originalFormation.AI.Side != FormationAI.BehaviorSide.BehaviorSideNotSet)
                            {
                                formation.AI.Side = originalFormation.AI.Side;

                            }
                            formation.SetMovementOrder(originalFormation.GetReadonlyMovementOrderReference());
                            formation.SetTargetFormation(originalFormation.TargetFormation);
                            formation.FacingOrder = originalFormation.FacingOrder;
                            formation.ArrangementOrder = originalFormation.ArrangementOrder;

                            //formation.SetPositioning(GetOrderPosition(formation), formation.Direction, formation.UnitSpacing);
                        }
                    }

                    SetMainAgentFormation(mission.MainAgent, formation);
                }
            }
        }

        public static bool IsInPlayerParty(Agent agent)
        {
            if (Campaign.Current != null)
            {
                if (agent.Origin is SimpleAgentOrigin simpleAgentOrigin && simpleAgentOrigin.Party == Campaign.Current.MainParty?.Party ||
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
            if (Mission.Current?.IsFastForward ?? false)
            {
                Mission.Current.SetFastForwardingFromUI(false);
            }
            var formation = agent.Formation;
            agent.Formation = null;
            agent.Controller = Agent.ControllerType.Player;
            // Note that the formation may be already set by SwitchFreeCameraLogic
            if (agent.Formation == null)
            {
                SetMainAgentFormation(agent, formation);
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
                if (mission.MainAgent.Controller == Agent.ControllerType.Player)
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

                    mission.MainAgent.Formation = null;
                    mission.MainAgent.Controller = Agent.ControllerType.AI;
                    // Note that the formation may be already set by SwitchFreeCameraLogic
                    if (mission.MainAgent.Formation == null)
                    {
                        SetMainAgentFormation(mission.MainAgent, formation);
                    }
                    // the Initialize method need to be called manually.
                    mission.MainAgent.CommonAIComponent?.Initialize();

                    // TODO: Need to check whether the following is needed.
                    mission.MainAgent.ResetEnemyCaches();
                    mission.MainAgent.InvalidateTargetAgent();
                    mission.MainAgent.InvalidateAIWeaponSelections();
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

        private static readonly MethodInfo SetCameraElevation =
            typeof(MissionScreen).GetProperty("CameraElevation", BindingFlags.Instance | BindingFlags.Public)
                ?.GetSetMethod(true);

        private static readonly MethodInfo SetCameraBearing =
            typeof(MissionScreen).GetProperty("CameraBearing", BindingFlags.Instance | BindingFlags.Public)
                ?.GetSetMethod(true);

        private static readonly FieldInfo IsPlayerAgentAdded =
            typeof(MissionScreen).GetField("_isPlayerAgentAdded", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool ShouldSmoothMoveToAgent = true;

        public static bool BeforeSetMainAgent()
        {
            if (ShouldSmoothMoveToAgent)
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
                        var targetFrame = GetCameraFrameWhenLockedToAgent(missionScreen, spectatingData.AgentToFollow, spectatingData.CameraType);
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
                if (changeElevation)
                {
                    CameraSpecialCurrentAddedElevation?.SetValue(missionScreen, missionScreen.CameraElevation - elevation);
                    SetCameraElevation?.Invoke(missionScreen, new object[] { elevation });
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

        public static MatrixFrame GetCameraFrameWhenLockedToAgent(MissionScreen missionScreen, Agent agentToFollow, SpectatorCameraTypes cameraType)
        {
            MatrixFrame result = MatrixFrame.Identity;
            float cameraBaseDistance = 0.6f;
            float agentScale = agentToFollow.AgentScale;
            if (agentToFollow.HasMount)
            {
                cameraBaseDistance += 0.1f;
            }
            if ((!missionScreen.IsViewingCharacter() && !(cameraType == SpectatorCameraTypes.LockToTeamMembersView && agentToFollow != missionScreen.Mission.MainAgent)) || missionScreen.IsPhotoModeEnabled)
            {
                cameraBaseDistance += 0.5f;
            }
            result.rotation.RotateAboutSide(1.57079637f);
            result.rotation.RotateAboutForward(missionScreen.CameraBearing);
            result.rotation.RotateAboutSide(missionScreen.CameraElevation);
            if (missionScreen.IsPhotoModeEnabled)
            {
                float a = -missionScreen.Mission.Scene.GetPhotoModeRoll();
                result.rotation.RotateAboutUp(a);
            }
            MatrixFrame matrixFrame = result;
            if (!missionScreen.IsPhotoModeEnabled)
                result.rotation.RotateAboutSide((float?)CameraAddedElevation?.GetValue(missionScreen) ?? 0);
            bool flag6 = agentToFollow.AgentVisuals != null && (uint)agentToFollow.AgentVisuals.GetSkeleton().GetCurrentRagdollState() > 0;
            var agentVisualPosition = agentToFollow.VisualPosition;
            var cameraTarget = flag6 ? agentToFollow.AgentVisuals.GetFrame().origin : agentVisualPosition;
            if (agentToFollow.HasMount)
            {
                var vec2_6 = agentToFollow.MountAgent.GetMovementDirection() * agentToFollow.MountAgent.Monster.RiderBodyCapsuleForwardAdder;
                cameraTarget += vec2_6.ToVec3();
            }
            
            cameraTarget.z += (float)CameraTargetAddedHeight.GetValue(missionScreen);
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
    }
}
