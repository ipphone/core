using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Plugins.ContactsUi.Controls
{
  public class DataGridViewTagsColumns : DataGridViewColumn
    {
        public DataGridViewTagsColumns()
            : base(new DataGridViewTagsCell())
        { }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewTagsCell)))
                {
                    throw new InvalidCastException("Must be a DataGridViewTagsCell");
                }

                base.CellTemplate = value;
            }
        }
    }

    public class DataGridViewTagsCell : DataGridViewCell
    {
        private readonly TagsListControl _tagsControl;
        private string _dataPropertyName;

        public override Type ValueType
        {
            get
            {
                return typeof(IEnumerable<IContactTagLocal>);
            }
        }

        public override bool ReadOnly
        {
            get
            {
                return true;
            }
        }

        public string DataPropertyName
        {
            get { return _dataPropertyName; }
            set
            {
                _dataPropertyName = value;

                OnDataGridViewChanged();
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                return typeof(string);
            }
        }

        public DataGridViewTagsCell()
        {
            _tagsControl = new TagsListControl();

            OnDataGridViewChanged();
        }
    }
}
