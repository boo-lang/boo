//
// (C) Paul Tingey 2004 
//
using System;
using System.Collections;
using System.ComponentModel;

namespace NDoc.Core.PropertyGridUI
{
    /// <summary>
    /// 
    /// </summary>
	public class PropertySorter : ExpandableObjectConverter
    {
		/// <summary>
		/// Gets whether the GetProperties method is supported.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <returns></returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context) 
        {
            return true;
        }

		/// <summary>
		/// Gets the properties.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="value">Value.</param>
		/// <param name="attributes">Attributes.</param>
		/// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            //
            // This override returns a list of properties in order
            //
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);
            ArrayList orderedProperties = new ArrayList();
            foreach (PropertyDescriptor pd in pdc)
            {
                Attribute attribute = pd.Attributes[typeof(PropertyOrderAttribute)];
                if (attribute != null)
                {
                    //
                    // If the attribute is found, then create an pair object to hold it
                    //
                    PropertyOrderAttribute poa = (PropertyOrderAttribute)attribute;
                    orderedProperties.Add(new PropertyOrderPair(pd.Name, poa.Order));
                }
                else
                {
                    //
                    // If no order attribute is specifed then given it an order of 0
                    //
                    orderedProperties.Add(new PropertyOrderPair(pd.Name, 0));
                }
            }
            //
            // Perform the actual order using the value PropertyOrderPair classes
            // implementation of IComparable to sort
            //
            orderedProperties.Sort();
            //
            // Build a string list of the ordered names
            //
            ArrayList propertyNames = new ArrayList();
            foreach (PropertyOrderPair pop in orderedProperties)
            {
                propertyNames.Add(pop.Name);
            }
            //
            // Pass in the ordered list for the PropertyDescriptorCollection to sort by
            //
            return pdc.Sort((string[])propertyNames.ToArray(typeof(string)));
        }

		#region Helper Class - PropertyOrderPair
		/// <summary>
		/// 
		/// </summary>
		internal class PropertyOrderPair : IComparable
		{
			private int _order;
			private string _name;
			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value></value>
			public string Name
			{
				get
				{
					return _name;
				}
			}

			/// <summary>
			/// Creates a new <see cref="PropertyOrderPair"/> instance.
			/// </summary>
			/// <param name="name">Name.</param>
			/// <param name="order">Order.</param>
			public PropertyOrderPair(string name, int order)
			{
				_order = order;
				_name = name;
			}

			/// <summary>
			/// Compares to.
			/// </summary>
			/// <param name="obj">Obj.</param>
			/// <returns></returns>
			public int CompareTo(object obj)
			{
				//
				// Sort the pair objects by ordering by order value
				// Equal values get the same rank
				//
				int otherOrder = ((PropertyOrderPair)obj)._order;
				if (otherOrder == _order)
				{
					//
					// If order not specified, sort by name
					//
					string otherName = ((PropertyOrderPair)obj)._name;
					return string.Compare(_name, otherName);
				}
				else if (otherOrder > _order)
				{
					return - 1;
				}
				return 1;
			}
		}
		#endregion
	}

    #region PropertyOrderAttribute

    /// <summary>
    /// 
    /// </summary>
	[AttributeUsage(AttributeTargets.Property)]
    public class PropertyOrderAttribute : Attribute
    {
        //
        // Simple attribute to allow specification of the order of a property in a propertgrid. 
        //
        private int _order;
		
		/// <summary>
		/// Creates a new <see cref="PropertyOrderAttribute"/> instance.
		/// </summary>
		/// <param name="order">Order.</param>
        public PropertyOrderAttribute(int order)
        {
            _order = order;
        }

		/// <summary>
		/// Gets the order.
		/// </summary>
		/// <value></value>
        public int Order
        {
            get
            {
                return _order;
            }
        }
    }
    #endregion

}
