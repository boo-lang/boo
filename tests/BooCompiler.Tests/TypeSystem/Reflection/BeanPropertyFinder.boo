namespace BooCompiler.Tests.TypeSystem.Reflection

import System
import System.Collections.Generic
import Boo.Lang.Compiler.TypeSystem

internal class BeanPropertyFinder:
	private final _properties = Dictionary[of string, BeanProperty]()

	def constructor(members as (IEntity)):
		for entity as IEntity in members:
			if entity.EntityType == EntityType.Method:
				if IsGetter(entity):
					ProcessBeanAccessor(entity as IMethod)
				elif IsSetter(entity):
					ProcessBeanMutator(entity as IMethod)

	private def IsSetter(entity as IEntity) as bool:
		return HasCamelCasePrefix(entity, "set")

	private def IsGetter(entity as IEntity) as bool:
		return HasCamelCasePrefix(entity, "get")

	private static def HasCamelCasePrefix(entity as IEntity, prefix as string) as bool:
		name = entity.Name
		return false if name.Length <= prefix.Length + 1
		return name.StartsWith(prefix) and char.IsUpper(name[prefix.Length])

	private def ProcessBeanAccessor(method as IMethod):
		BeanPropertyFor(method).Getter = method

	private def ProcessBeanMutator(method as IMethod):
		BeanPropertyFor(method).Setter = method

	private def BeanPropertyFor(method as IMethod) as BeanProperty:
		propertyName = method.Name.Substring(3)

		beanProperty as BeanProperty
		if not _properties.TryGetValue(propertyName, beanProperty):
			beanProperty = BeanProperty(propertyName)
			_properties.Add(propertyName, beanProperty)
		return beanProperty

	public def FindAll() as (IEntity):
		result = array(BeanProperty, _properties.Count)
		_properties.Values.CopyTo(result, 0)
		return result

internal class BeanProperty(IProperty):
	private _getter as IMethod
	private _setter as IMethod
	private _name as string

	def constructor(name as string):
		_name = char.ToLower(name[0]) + name.Substring(1)

	public Getter as IMethod:
		set:
			raise InvalidOperationException() if _getter is not null
			_getter = value

	public Setter as IMethod:
		set:
			raise InvalidOperationException() if _setter is not null
			_setter = value

	# Implementation of IEntity

	public Name as string:
		get: return _name

	public FullName as string:
		get: raise System.NotImplementedException()

	public EntityType as EntityType:
		get: raise System.NotImplementedException()

	# Implementation of ITypedEntity

	public Type as IType:
		get: raise System.NotImplementedException()

	# Implementation of IEntityWithAttributes

	public def IsDefined(attributeType as IType) as bool:
		raise System.NotImplementedException()

	# Implementation of IMember

	public IsDuckTyped as bool:
		get: raise System.NotImplementedException()

	public DeclaringType as IType:
		get: raise System.NotImplementedException()

	public IsStatic as bool:
		get: raise System.NotImplementedException()

	public IsPublic as bool:
		get: raise System.NotImplementedException()

	# Implementation of IAccessibleMember

	public IsProtected as bool:
		get: raise System.NotImplementedException()

	public IsInternal as bool:
		get: raise System.NotImplementedException()

	public IsPrivate as bool:
		get: raise System.NotImplementedException()

	# Implementation of IEntityWithParameters

	public def GetParameters() as (IParameter):
		raise System.NotImplementedException()

	public AcceptVarArgs as bool:
		get: raise System.NotImplementedException()

	# Implementation of IExtensionEnabled

	public IsExtension as bool:
		get: raise System.NotImplementedException()

	# Implementation of IProperty

	public def GetGetMethod() as IMethod:
		return _getter

	public def GetSetMethod() as IMethod:
		return _setter
