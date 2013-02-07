using System;
using System.Linq;

using AluminumLua;

namespace Autofac.AluminumLua
{
	public class AutofacLuaReflection
	{
		public object Resolve(global::Autofac.IComponentContext context, object type)
		{
			Type t = AutofacLuaBuilder.ResolveTypeFromLuaUserObject(type);
			return context.Resolve(t);
		}
		public LuaObject Activate(LuaObject[] args)
		{
			Type t = AutofacLuaBuilder.ResolveTypeFromLuaUserObject(args[0].AsUserData());
			return LuaObject.FromUserData(Activator.CreateInstance(t, (from a in args select a.AsUserData()).Skip(1).ToArray()));
		}

		public LuaObject Invoke(LuaObject[] args)
		{
			var obj = args[0].AsUserData();
			var name = args[1].AsString();
			var m = obj.GetType().GetMethod(name, (from a in args select a.AsUserData().GetType()).Skip(2).ToArray());
			return LuaObject.FromUserData(m.Invoke(obj, (from a in args select a.AsUserData()).Skip(2).ToArray()));
		}

		public object Type(object arg)
		{
			Type t = AutofacLuaBuilder.ResolveTypeFromLuaUserObject(arg);
			return LuaObject.FromUserData(t);
		}

		public LuaObject GetValue(LuaObject[] args)
		{
			object target = null;
			string name = null;
			object []index =null;
			if (args.Length > 0) target = args[0].AsUserData();
			if (args.Length > 1) name = args[1].AsString();

			var type = target.GetType();
			var prop = type.GetProperty(name);
			if (prop!=null)
			{
				return LuaObject.FromObject(prop.GetValue(target, index));
			}
			var field = type.GetField(name);
			if (field != null)
			{
				return LuaObject.FromObject(field.GetValue(target));
			}
			throw new ArgumentException(string.Format("Filed {0} not found in {1}", name, type.FullName));
		}

		public LuaObject SetValue(LuaObject[] args)
		{
			object target = null;
			string name = null;
			object[] index = null;
			if (args.Length > 0) target = args[0].AsUserData();
			if (args.Length > 1) name = args[1].AsString();
			object value = args[args.Length - 1];

			var type = target.GetType();
			var prop = type.GetProperty(name);
			if (prop != null)
			{
				prop.SetValue(target, Convert.ChangeType(value,prop.PropertyType), index);
				return LuaObject.FromObject(target);
			}
			var field = type.GetField(name);
			if (field != null)
			{
				field.SetValue(target, Convert.ChangeType(value, field.FieldType));
				return LuaObject.FromObject(target);
			}
			throw new ArgumentException(string.Format("Filed {0} not found in {1}", name, type.FullName));
		}
		public object TypeOf(object arg)
		{
			if (arg == null) return LuaObject.Nil;
			Type t = arg.GetType();
			return LuaObject.FromUserData(t);
		}
	}
}