using System;
using System.Reflection;
namespace TalentSystemDefaults
{
	public class ReflectionUtils
	{
		public static System.Type[] GetTypes()
		{
			return Assembly.GetExecutingAssembly().GetTypes();
		}
		public static Type GetTypeInfo(string className)
		{
			Type type = Assembly.GetExecutingAssembly().GetType(className, true, true);
			return type;
		}
		public static dynamic CreateInstance(string className, object[] parameters)
		{
			object instance = null;
			Type type = GetTypeInfo(className);
			ConstructorInfo ctor = type.GetConstructors()[0];
			instance = ctor.Invoke(parameters);
			return instance;
		}
	}
}
