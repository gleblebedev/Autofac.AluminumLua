using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AluminumLua;

using Autofac.Builder;
using Autofac.Core;

namespace Autofac.AluminumLua
{
	public class AutofacLuaBuilder
	{
		private readonly ContainerBuilder builder;

		public AutofacLuaBuilder(ContainerBuilder builder)
		{
			this.builder = builder;
		}
		public object Register(LuaFunction o)
		{
			Func<IComponentContext, IEnumerable<Parameter>, object> d = (a, p) => this.Call(o, a, p);
			var rb = RegistrationBuilder.ForDelegate(d);
			this.builder.RegisterCallback((Action<IComponentRegistry>)(cr => RegistrationBuilder.RegisterSingleComponent<object, SimpleActivatorData, SingleRegistrationStyle>(cr, rb)));
			return rb;
		}

		private object Call(LuaFunction luaFunction, IComponentContext componentContext, IEnumerable<Parameter> parameters)
		{
			var args = new List<LuaObject>();
			args.Add(LuaObject.FromUserData(componentContext));
			args.AddRange(from p in parameters select LuaObject.FromUserData(p));
			LuaObject objects = luaFunction.Invoke(args.ToArray());
			return objects.AsUserData();
		}
		public object RegisterInstance(object type)
		{
			return this.builder.RegisterInstance(type);
		}

		public void RegisterModule(object type)
		{
			if (type is Type)
			{
				this.builder.RegisterModule((IModule)Activator.CreateInstance((Type)type));
			}
			else if (type is IModule)
			{
				this.builder.RegisterModule((IModule)type);
			}
			else
			{
				throw new ApplicationException(type + " isn't autofac module");
			}
		}

		public object Resolve(global::Autofac.IComponentContext context, object type)
		{
			Type t = ResolveTypeFromLuaUserObject(type);
			return context.Resolve(t);
		}

		public object As(object rb, object type)
		{
			Type t = ResolveTypeFromLuaUserObject(type);

			//(new global::Autofac.ContainerBuilder()).RegisterType<object>().As(typeof(int));
			var m = rb.GetType().GetMethod("As", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(Type[]) }, null);
			return m.Invoke(rb, new object[] { new Type[] { t } });
		}

		private static Type ResolveTypeFromLuaUserObject(object type)
		{
			Type t;
			if (type is string)
			{
				t = System.Type.GetType((string)type);
			}
			else if (type is Type)
			{
				t = (Type)type;
			}
			else
			{
				throw new ApplicationException(string.Format("{0} isn't type", type));

			}
			return t;
		}

		public object SingleInstance(object rb)
		{
			var m = rb.GetType().GetMethod("SingleInstance", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { }, null);
			return m.Invoke(rb, new object[] { });
		}
		public object InstancePerDependency(object rb)
		{
			var m = rb.GetType().GetMethod("InstancePerDependency", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { }, null);
			return m.Invoke(rb, new object[] { });
		}
		public object RegisterGeneric(object type)
		{
			if (type is Type)
			{
				return this.builder.RegisterGeneric((Type)type);
			}
			//if (type is ProxyType)
			//{
			//    return builder.RegisterGeneric(((ProxyType)type).UnderlyingSystemType);
			//}
			if (type is string)
			{
				return this.builder.RegisterGeneric(System.Type.GetType((string)type));
			}
			throw new ApplicationException(type + " isn't type");
		}
		public object RegisterType(object type)
		{
			Type t = ResolveTypeFromLuaUserObject(type);
			return this.builder.RegisterType(t);
		}

		public LuaObject Activate(LuaObject[] args)
		{
			Type t = ResolveTypeFromLuaUserObject(args[0].AsUserData());
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
			Type t = ResolveTypeFromLuaUserObject(arg);
			return LuaObject.FromUserData(t);
		}

		public object TypeOf(object arg)
		{
			if (arg == null) return LuaObject.Nil;
			Type t = arg.GetType();
			return LuaObject.FromUserData(t);
		}
	}
}