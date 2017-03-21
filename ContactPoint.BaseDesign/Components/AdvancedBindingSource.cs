using System;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace ContactPoint.BaseDesign.Components
{
    public class AdvancedBindingSource<T> : BindingSource
    {
        public new T Current
        {
            get { return (T)base.Current; }
        }

        public AdvancedBindingSource()
        {
            this.DataSource = typeof (T);
        }

        /// <summary>
        /// Creates a DataBinding between a control property and a datasource property
        /// </summary>
        /// <typeparam name="TControl">The control type, must derive from System.Winforms.Control</typeparam>
        /// <param name="controlInstance">The control instance on wich the databinding will be added</param>
        /// <param name="controlPropertyAccessor">A lambda expression which specifies the control property to be databounded (something like textboxCtl => textboxCtl.Text)</param>
        /// <param name="datasourceMemberAccesor">A lambda expression which specifies the datasource property (something like datasource => datasource.Property) </param>
        public void CreateBinding<TControl>(TControl controlInstance, Expression<Func<TControl, object>> controlPropertyAccessor, Expression<Func<T, object>> datasourceMemberAccesor)
            where TControl : Control
        {
            this.CreateBinding(controlInstance, controlPropertyAccessor, datasourceMemberAccesor, DataSourceUpdateMode.OnValidation, "");
        }

        /// <summary>
        /// Creates a DataBinding between a control property and a datasource property
        /// </summary>
        /// <typeparam name="TControl">The control type, must derive from System.Winforms.Control</typeparam>
        /// <param name="controlInstance">The control instance on wich the databinding will be added</param>
        /// <param name="controlPropertyAccessor">A lambda expression which specifies the control property to be databounded (something like textboxCtl => textboxCtl.Text)</param>
        /// <param name="datasourceMemberAccesor">A lambda expression which specifies the datasource property (something like datasource => datasource.Property) </param>
        /// <param name="format">Format for formatted binding</param>
        public void CreateBinding<TControl>(TControl controlInstance, Expression<Func<TControl, object>> controlPropertyAccessor, Expression<Func<T, object>> datasourceMemberAccesor, string format)
            where TControl : Control
        {
            this.CreateBinding(controlInstance, controlPropertyAccessor, datasourceMemberAccesor, DataSourceUpdateMode.OnValidation, format);
        }

        /// <summary>
        /// Creates a DataBinding between a control property and a datasource property
        /// </summary>
        /// <typeparam name="TControl">The control type, must derive from System.Winforms.Control</typeparam>
        /// <param name="controlInstance">The control instance on wich the databinding will be added</param>
        /// <param name="controlPropertyAccessor">A lambda expression which specifies the control property to be databounded (something like textboxCtl => textboxCtl.Text)</param>
        /// <param name="datasourceMemberAccesor">A lambda expression which specifies the datasource property (something like datasource => datasource.Property) </param>
        /// <param name="dataSourceUpdateMode">See control.DataBindings.Add method </param>
        /// <param name="format">Format for formatted binding</param>
        public void CreateBinding<TControl>(TControl controlInstance, Expression<Func<TControl, object>> controlPropertyAccessor, Expression<Func<T, object>> datasourceMemberAccesor, DataSourceUpdateMode dataSourceUpdateMode, string format)
            where TControl : Control
        {
            string controlPropertyName = PropertyAccessor.GetPropertyName(controlPropertyAccessor);
            string sourcePropertyName = PropertyAccessor.GetPropertyName(datasourceMemberAccesor);

            controlInstance.DataBindings.Add(controlPropertyName, this, sourcePropertyName, true, dataSourceUpdateMode, "", format);
        }
    }
}
