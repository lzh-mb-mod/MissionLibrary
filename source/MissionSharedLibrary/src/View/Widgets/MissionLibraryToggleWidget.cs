// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party.PartyHeaderToggleWidget
// Assembly: TaleWorlds.MountAndBlade.GauntletUI.Widgets, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AA730C31-ADBC-4DB4-BC16-0D57CCDB38EB
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.GauntletUI.Widgets.dll

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;

#nullable disable
namespace MissionSharedLibrary.View.Widgets
{
    public class MissionLibraryToggleWidget : ToggleButtonWidget
    {
        private int _latestChildCount;
        private ListPanel _listPanel;
        private ButtonWidget _transferButtonWidget;
        private BrushWidget _collapseIndicator;
        //private bool _isRelevant = true;
        //private bool _blockInputsWhenDisabled;

        public MissionLibraryToggleWidget(UIContext context) : base(context)
        {
        }

        public bool AutoToggleTransferButtonState { get; set; } = true;

        protected override void OnClick(Widget widget)
        {
            if (/*this.BlockInputsWhenDisabled && */this._listPanel != null && this._listPanel.ChildCount <= 0)
                return;
            base.OnClick(widget);
            this.UpdateCollapseIndicator();
        }

        private void OnListSizeChange(Widget widget) => this.UpdateSize();

        private void OnListSizeChange(Widget parentWidget, Widget addedWidget) => this.UpdateSize();


        protected override void OnConnectedToRoot()
        {
            base.OnConnectedToRoot();
        }

        public override void SetState(string stateName)
        {
            if (/*this.BlockInputsWhenDisabled && */this._listPanel != null && this._listPanel.ChildCount <= 0)
                return;
            base.SetState(stateName);
        }

        private void UpdateSize()
        {
            if (this.TransferButtonWidget != null && this.AutoToggleTransferButtonState)
                this.TransferButtonWidget.IsEnabled = this._listPanel.ChildCount > 0;
            //if (this.IsRelevant)
            {
                this.IsVisible = true;
                //if (this._listPanel.ChildCount > 0)
                //    this._listPanel.IsVisible = true;
                //if (this._listPanel.ChildCount > this._latestChildCount && !this.WidgetToClose.IsVisible)
                    //this.HandleClick();
            }
            //else
            //    this._listPanel.IsVisible = false;
            this._latestChildCount = this._listPanel.ChildCount;
            this.UpdateCollapseIndicator();
        }

        private void ListPanelUpdated()
        {
            if (this.TransferButtonWidget != null)
                this.TransferButtonWidget.IsEnabled = false;
            this._listPanel.ItemAfterRemoveEventHandlers.Add(new Action<Widget>(this.OnListSizeChange));
            this._listPanel.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnListSizeChange));
            this.UpdateSize();
        }

        private void TransferButtonUpdated() => this.TransferButtonWidget.IsEnabled = false;

        private void CollapseIndicatorUpdated()
        {
            this.CollapseIndicator.AddState("Collapsed");
            this.CollapseIndicator.AddState("Expanded");
            this.UpdateCollapseIndicator();
        }

        private void UpdateCollapseIndicator()
        {
            if (this.WidgetToClose == null || this.CollapseIndicator == null)
                return;
            if (this.WidgetToClose.IsVisible)
                this.CollapseIndicator.SetState("Expanded");
            else
                this.CollapseIndicator.SetState("Collapsed");
        }

        [Editor(false)]
        public ListPanel ListPanel
        {
            get => this._listPanel;
            set
            {
                if (this._listPanel == value)
                    return;
                this._listPanel = value;
                this.OnPropertyChanged<ListPanel>(value, nameof(ListPanel));
                this.ListPanelUpdated();
            }
        }

        [Editor(false)]
        public ButtonWidget TransferButtonWidget
        {
            get => this._transferButtonWidget;
            set
            {
                if (this._transferButtonWidget == value)
                    return;
                this._transferButtonWidget = value;
                this.OnPropertyChanged<ButtonWidget>(value, nameof(TransferButtonWidget));
                this.TransferButtonUpdated();
            }
        }

        [Editor(false)]
        public BrushWidget CollapseIndicator
        {
            get => this._collapseIndicator;
            set
            {
                if (this._collapseIndicator == value)
                    return;
                this._collapseIndicator = value;
                this.OnPropertyChanged<BrushWidget>(value, nameof(CollapseIndicator));
                this.CollapseIndicatorUpdated();
            }
        }
    }

}
