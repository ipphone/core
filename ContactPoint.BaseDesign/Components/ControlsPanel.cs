using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace ContactPoint.BaseDesign.Components
{
    public class ControlsPanel<T, TControl> : SelectableVerticalLayoutList
        where T : class
        where TControl : Control, IEntityControl<T>, new()
    {
        private readonly ListChangedEventHandler _listChangedHandler;
        private readonly EventHandler _positionChangedHandler;
        private object _dataSource;
        private CurrencyManager _dataManager;
        private string _dataMember;
        private BindingList<T> _items = new BindingList<T>();

        public event EventHandler ControlDoubleClick;

        [ReadOnly(true)]
        public IList<T> Items
        {
            get { return _items; }
        }

        public T SelectedEntity
        {
            get
            {
                if (SelectedControl == null) return null;

                return (SelectedControl as IEntityControl<T>).Entity;
            }
        }

        #region DataSource

        [ReadOnly(true)]
        public object DataSource
        {
            get
            {
                return this._dataSource;
            }
            set
            {
                if (this._dataSource != value)
                {
                    this._dataSource = value;
                    
                    TryDataBinding();
                }
            }
        }

        public string DataMember
        {
            get { return this._dataMember; }
            set
            {
                if (this._dataMember != value)
                {
                    this._dataMember = value;

                    TryDataBinding();
                }
            }
        }

        #endregion

        public ControlsPanel()
        {
            _listChangedHandler = new ListChangedEventHandler(dataManager_ListChanged);
            _positionChangedHandler = new EventHandler(dataManager_PositionChanged);

            _items.ListChanged += _listChangedHandler;

            if (!DesignMode) DataSource = Items;
        }

        protected override void OnBindingContextChanged(EventArgs e)
        {
            TryDataBinding();

            base.OnBindingContextChanged(e);
        }

        protected void OnControlDoubleClick(EventArgs e)
        {
            if (ControlDoubleClick != null)
                ControlDoubleClick(this, e);
        }

        private void TryDataBinding()
        {
            if (BindingContext == null || DataSource == null)
                return;

            CurrencyManager cm;
            try
            {
                cm = (CurrencyManager)BindingContext[this.DataSource, this.DataMember];
            }
            catch { return; }

            if (this._dataManager != cm)
            {
                // Unwire the old CurrencyManager
                if (this._dataManager != null)
                {
                    this._dataManager.ListChanged -= _listChangedHandler;
                    this._dataManager.PositionChanged -= _positionChangedHandler;
                }
                this._dataManager = cm;

                // Wire the new CurrencyManager
                this._dataManager.ListChanged += _listChangedHandler;
                this._dataManager.PositionChanged += _positionChangedHandler;

                // Update metadata and data
                UpdateAllData();
            }
        }

        private void UpdateAllData()
        {
            for (int i = 0; i < this.Controls.Count; i++)
            {
                this.Controls[i].GotFocus -= control_GotFocus;
                this.Controls[i].DoubleClick -= control_DoubleClick;
            }

            this.Controls.Clear();

            if (_dataManager != null)
                for (int i = 0; i < _dataManager.Count; i++)
                    AddItem(_dataManager.List[i] as T);
        }

        void control_DoubleClick(object sender, EventArgs e)
        {
            OnControlDoubleClick(e);
        }

        void control_GotFocus(object sender, EventArgs e)
        {
            var control = sender as TControl;

            if (control != null)
                this._dataManager.Position = this._dataManager.List.IndexOf(control.Entity);
        }

        #region Items functions

        private void AddItem(T entity)
        {
            if (entity == null)
                return;

            var control = new TControl {Entity = entity};
            control.DoubleClick += control_DoubleClick;
            control.GotFocus += control_GotFocus;

            this.Controls.Add(control);
        }

        #endregion

        #region DataManager events

        private void dataManager_PositionChanged(object sender, EventArgs e)
        {
            // This code may cycle control
            //foreach (TControl control in this.Controls)
            //    if (control.Entity == _dataManager.Current)
            //        control.Focus();
        }

        private void dataManager_ListChanged(object sender, ListChangedEventArgs e)
        {
            UpdateAllData();
        }

        #endregion
    }
}
