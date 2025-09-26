using System;
using System.Collections.Generic;

namespace MissionSharedLibrary.View.ViewModelCollection.Options.Selection
{
    public class SelectionOptionData
    {
        private readonly Action<int> _setValue;
        private readonly Func<int> _getValue;
        private int _value;
        private readonly Func<int> _limit;
        private readonly Func<IEnumerable<SelectionItem>> _data;

        public SelectionOptionData(Action<int> setValue, Func<int> getValue, Func<int> limit, Func<IEnumerable<SelectionItem>> data)
        {
            _setValue = setValue;
            _getValue = getValue;
            _value = getValue();
            _limit = limit;
            _data = data;
        }

        public int GetDefaultValue()
        {
            return _getValue();
        }

        public void Commit()
        {
            _setValue(_value);
        }

        public int GetValue()
        {
            return _value;
        }

        public void SetValue(int value)
        {
            _value = value;
        }

        public int GetSelectableOptionsLimit()
        {
            return _limit();
        }

        public IEnumerable<SelectionItem> GetSelectableOptionNames()
        {
            return _data();
        }
    }
}
