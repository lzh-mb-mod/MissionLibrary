using MissionLibrary.HotKey;
using MissionSharedLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.Config.HotKey
{
    public class GameKeySequenceAlternative
    {
        public List<Key> Keys { get; set; }= new List<Key>();

        private int _progress = 0;

        public GameKeySequenceAlternative(List<InputKey> keys)
        {
            Keys = keys.Select(key => new Key(key)).ToList();
        }

        public SerializedGameKeySequenceAlternative ToSerializedGameKeySequenceAlternative()
        {
            return new SerializedGameKeySequenceAlternative
            {
                KeyboardKeys = Keys.Select(key => key.InputKey).Where(inputKey => inputKey != InputKey.Invalid).ToList(),
            };
        }

        public void SetGameKeys(List<InputKey> inputKeys)
        {
            var keys = inputKeys.Where(key => key != InputKey.Invalid).Select(inputKey => new Key(inputKey)).ToList();
            if (keys.Count == 0)
                return;

            Keys = keys;
        }

        public bool IsKeyDownInOrder(IInputContext input = null)
        {
            if (!CheckCurrentProgress(input))
                return false;

            for (int i = _progress; i < Keys.Count; ++i)
            {
                if (IsKeyDown(input, i))
                    ++_progress;
                else
                    return false;
            }

            return true;
        }

        public bool IsKeyPressedInOrder(IInputContext input = null)
        {
            if (!CheckCurrentProgress(input))
                return false;

            for (int i = _progress; i < Keys.Count - 1; ++i)
            {
                if (IsKeyDown(input, i))
                    ++_progress;
                else
                    return false;
            }

            return IsKeyPressed(input, Keys.Count - 1);
        }

        public bool IsKeyReleasedInOrder(IInputContext input = null)
        {
            if (!CheckCurrentProgress(input))
                return false;

            for (int i = _progress; i < Keys.Count - 1; ++i)
            {
                if (IsKeyDown(input, i))
                    ++_progress;
                else
                    return false;
            }

            return IsKeyReleased(input, Keys.Count - 1);
        }

        public bool IsKeyDown(IInputContext input = null)
        {
            if (Keys.Count == 0)
                return false;

            for (int i = 0; i < Keys.Count; ++i)
            {
                if (!IsKeyDown(input, i))
                    return false;
            }

            return true;
        }

        public bool IsKeyPressed(IInputContext input = null)
        {
            if (Keys.Count == 0)
                return false;

            for (int i = 0; i < Keys.Count - 1; ++i)
            {
                if (!IsKeyDown(input, i))
                    return false;
            }

            return IsKeyPressed(input, Keys.Count - 1);
        }

        public bool IsKeyReleased(IInputContext input = null)
        {
            if (Keys.Count == 0)
                return false;

            for (int i = 0; i < Keys.Count - 1; ++i)
            {
                if (!IsKeyDown(input, i))
                    return false;
            }

            return IsKeyReleased(input, Keys.Count - 1);
        }

        private bool CheckCurrentProgress(IInputContext input)
        {
            if (Keys == null || Keys.Count == 0)
                return false;
            for (int i = 0; i < _progress; ++i)
            {
                if (!IsKeyDown(input, i))
                {
                    _progress = i;
                    return false;
                }
            }

            return true;
        }

        private bool IsKeyDown(IInputContext input, int i)
        {
            return input?.IsKeyDown(Keys[i].InputKey) ?? Input.IsKeyDown(Keys[i].InputKey);
        }

        private bool IsKeyPressed(IInputContext input, int i)
        {
            return input?.IsKeyPressed(Keys[i].InputKey) ?? Input.IsKeyPressed(Keys[i].InputKey);
        }

        private bool IsKeyReleased(IInputContext input, int i)
        {
            return input?.IsKeyReleased(Keys[i].InputKey) ?? Input.IsKeyReleased(Keys[i].InputKey);
        }
        public string ToHintString()
        {
            if (Keys.Count == 0)
            {
                return "[No key]";
            }
            string result = "";
            for (int i = 0; i < Keys.Count - 1; ++i)
            {
                result += Utility.TextForKey(Keys[i].InputKey) + "+";
            }

            result += Utility.TextForKey(Keys[Keys.Count - 1].InputKey);
            return result;
        }
    }

    public class GameKeySequence : IGameKeySequence
    {
        public int Id;
        public string StringId;
        public string CategoryId;

        public List<GameKeySequenceAlternative> KeyAlternatives;

        public bool Mandatory = false;

        private readonly List<GameKeySequenceAlternative> _defaultGameKeys;
        private readonly List<GameKeySequenceAlternative> _forbiddenGameKeys;

        public GameKeySequence(int id, string stringId, string categoryId, List<GameKeySequenceAlternative> sequenceAlternatives, bool mandatory = false, List<GameKeySequenceAlternative> forbiddenAlternatives = null)
        {
            Id = id;
            StringId = stringId;
            CategoryId = categoryId;
            Mandatory = mandatory;
            _defaultGameKeys = sequenceAlternatives;
            _forbiddenGameKeys = forbiddenAlternatives ?? new List<GameKeySequenceAlternative>();
            sequenceAlternatives = NormalizeGameKeySequenceAlternatives(sequenceAlternatives);
            SetGameKeys(sequenceAlternatives);
        }

        public SerializedGameKeySequence ToSerializedGameKeySequence()
        {
            return new SerializedGameKeySequence
            {
                StringId = StringId,
                GameKeyAlternatives = NormalizeGameKeySequenceAlternatives(KeyAlternatives).Select(sa => new SerializedGameKeySequenceAlternative { KeyboardKeys = sa.Keys.Select(key => key.InputKey).ToList()}).ToList()
            };
        }

        public void SetGameKeys(List<GameKeySequenceAlternative> inputKeys)
        {
            var keys = NormalizeGameKeySequenceAlternatives(inputKeys);
            if (Mandatory && keys.Count == 0)
            {
                ResetToDefault();
                return;
            }
            KeyAlternatives = keys;
        }

        public void ClearInvalidKeys()
        {
            KeyAlternatives = NormalizeGameKeySequenceAlternatives(KeyAlternatives);
        }

        public void ResetToDefault()
        {
            SetGameKeys(_defaultGameKeys);
        }

        public string ToSequenceString()
        {
            var list = KeyAlternatives.Where(sa => sa.Keys.Any()).Select(sa => sa.ToHintString()).ToList();

            if (list.Count == 0)
            {
                return "[No key]";
            }

            return String.Join(Module.CurrentModule.GlobalTextManager.FindText("str_mission_library_game_key_or").ToString(), list);
        }

        private List<GameKeySequenceAlternative> NormalizeGameKeySequenceAlternatives(List<GameKeySequenceAlternative> alternatives)
        {
            var result = new List<GameKeySequenceAlternative>();
            foreach (var alternative in alternatives)
            {
                var newSequenceAlternative = new GameKeySequenceAlternative(alternative.Keys.Where(key => key.InputKey != InputKey.Invalid).Select(key => key.InputKey).ToList());
                // key sequence should not be empty.
                if (!newSequenceAlternative.Keys.Any())
                    continue;
                // key sequence should not be the same as any forbidden key sequences.
                if (IsSequenceAlternativeForbidden(newSequenceAlternative))
                    continue;
                result.Add(newSequenceAlternative);
            }
            return result;
        }
        
        private bool IsSequenceAlternativeForbidden(GameKeySequenceAlternative sequenceAlternative)
        {
            foreach (var forbiddenKeySequence in _forbiddenGameKeys)
            {
                if (sequenceAlternative.Keys.Select(key => key.InputKey).SequenceEqual(forbiddenKeySequence.Keys.Select(key => key.InputKey)))
                    return true;
            }
            return false;
        }

        public bool IsKeyDownInOrder(IInputContext input = null)
        {
            bool isKeyDownInOrder = false;
            for (int i = 0; i < KeyAlternatives.Count; i++)
            {
                isKeyDownInOrder |= KeyAlternatives[i].IsKeyDownInOrder(input);
            }
            return isKeyDownInOrder;
        }

        public bool IsKeyPressedInOrder(IInputContext input = null)
        {
            bool isKeyPressedInOrder = false;
            for (int i = 0; i < KeyAlternatives.Count; ++i)
            {
                isKeyPressedInOrder |= KeyAlternatives[i].IsKeyPressedInOrder(input);
            }
            return isKeyPressedInOrder;
        }

        public bool IsKeyReleasedInOrder(IInputContext input = null)
        {
            bool isKeyReleasedInOrder = false;
            for (int i = 0; i < KeyAlternatives.Count; ++i)
            {
                isKeyReleasedInOrder |= KeyAlternatives[i].IsKeyReleasedInOrder(input);
            }
            return isKeyReleasedInOrder;
        }

        public bool IsKeyDown(IInputContext input = null)
        {
            bool isKeyDown = false;
            for (int i = 0; i < KeyAlternatives.Count; ++i)
            {
                isKeyDown |= KeyAlternatives[i].IsKeyDown(input);
            }
            return isKeyDown;
        }

        public bool IsKeyPressed(IInputContext input = null)
        {
            bool isKeyPressed = false;
            for (int i = 0; i < KeyAlternatives.Count; ++i)
            {
                isKeyPressed |= KeyAlternatives[i].IsKeyPressed(input);
            }
            return isKeyPressed;
        }

        public bool IsKeyReleased(IInputContext input = null)
        {
            bool isKeyReleased = false;
            for (int i = 0;i < KeyAlternatives.Count; i++)
            {
                isKeyReleased |= KeyAlternatives[i].IsKeyReleased(input);
            }
            return isKeyReleased;
        }
    }
}
