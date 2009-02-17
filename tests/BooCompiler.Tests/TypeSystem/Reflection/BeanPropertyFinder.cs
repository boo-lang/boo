using System;
using System.Collections.Generic;
using Boo.Lang.Compiler.TypeSystem;

namespace BooCompiler.Tests.TypeSystem.Reflection
{
	internal class BeanPropertyFinder
	{
		Dictionary<string, BeanProperty> _properties = new Dictionary<string, BeanProperty>();

		public BeanPropertyFinder(IEntity[] members)
		{
			foreach (IEntity entity in members)
				if (entity.EntityType == EntityType.Method)
					if (IsGetter(entity))
						ProcessBeanAccessor((IMethod)entity);
					else if (IsSetter(entity))
						ProcessBeanMutator((IMethod) entity);
		}

		private bool IsSetter(IEntity entity)
		{
			return HasCamelCasePrefix(entity, "set");
		}

		private bool IsGetter(IEntity entity)
		{
			return HasCamelCasePrefix(entity, "get");
		}

		private bool HasCamelCasePrefix(IEntity entity, string prefix)
		{
			string name = entity.Name;
			if (name.Length <= prefix.Length + 1) return false;
			return name.StartsWith(prefix) && char.IsUpper(name[prefix.Length]);
		}

		private void ProcessBeanAccessor(IMethod method)
		{
			BeanPropertyFor(method).Getter = method;
		}

		private void ProcessBeanMutator(IMethod method)
		{
			BeanPropertyFor(method).Setter = method;
		}

		private BeanProperty BeanPropertyFor(IMethod method)
		{
			string propertyName = method.Name.Substring(3);

			BeanProperty beanProperty;
			if (!_properties.TryGetValue(propertyName, out beanProperty))
			{
				beanProperty = new BeanProperty(propertyName);
				_properties.Add(propertyName, beanProperty);
			}
			return beanProperty;
		}

		public IEntity[] FindAll()
		{
			BeanProperty[] result = new BeanProperty[_properties.Count];
			_properties.Values.CopyTo(result, 0);
			return result;
		}
	}

	internal class BeanProperty : IProperty
	{
		private IMethod _getter;
		private IMethod _setter;
		private string _name;

		public BeanProperty(string name)
		{
			_name = char.ToLower(name[0]) + name.Substring(1);
		}

		public IMethod Getter
		{
			set
			{
				if (_getter != null) throw new InvalidOperationException();
				_getter = value;
			}
		}

		public IMethod Setter
		{
			set
			{
				if (_setter != null) throw new InvalidOperationException();
				_setter = value;
			}
		}


		#region Implementation of IEntity

		public string Name
		{
			get { return _name; }
		}

		public string FullName
		{
			get { throw new System.NotImplementedException(); }
		}

		public EntityType EntityType
		{
			get { throw new System.NotImplementedException(); }
		}

		#endregion

		#region Implementation of ITypedEntity

		public IType Type
		{
			get { throw new System.NotImplementedException(); }
		}

		#endregion

		#region Implementation of IEntityWithAttributes

		public bool IsDefined(IType attributeType)
		{
			throw new System.NotImplementedException();
		}

		#endregion

		#region Implementation of IMember

		public bool IsDuckTyped
		{
			get { throw new System.NotImplementedException(); }
		}

		public IType DeclaringType
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsStatic
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsPublic
		{
			get { throw new System.NotImplementedException(); }
		}

		#endregion

		#region Implementation of IAccessibleMember

		public bool IsProtected
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsInternal
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsPrivate
		{
			get { throw new System.NotImplementedException(); }
		}

		#endregion

		#region Implementation of IEntityWithParameters

		public IParameter[] GetParameters()
		{
			throw new System.NotImplementedException();
		}

		public bool AcceptVarArgs
		{
			get { throw new System.NotImplementedException(); }
		}

		#endregion

		#region Implementation of IExtensionEnabled

		public bool IsExtension
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsBooExtension
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsClrExtension
		{
			get { throw new System.NotImplementedException(); }
		}

		#endregion

		#region Implementation of IProperty

		public IMethod GetGetMethod()
		{
			return _getter;
		}

		public IMethod GetSetMethod()
		{
			return _setter;
		}

		#endregion
	}
}