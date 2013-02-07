using System;
using System.Collections.Generic;

using AluminumLua;

namespace Autofac.AluminumLua
{
	/// <summary>
	/// The autofac lua module.
	/// </summary>
	public class AutofacLuaModule : Module
	{
		private string fileName;

		public AutofacLuaModule()
		{
		}

		public AutofacLuaModule(string fileName)
		{
			this.fileName = fileName;
		}

		#region Methods

		/// <summary>
		/// The load.
		/// </summary>
		/// <param name="builder">
		/// The builder.
		/// </param>
		protected override void Load(ContainerBuilder builder)
		{
			if (string.IsNullOrEmpty(this.fileName)) this.fileName = "autofac.lua";

			LuaParser parser;

			var context = new LuaContext();
			context.AddBasicLibrary();
			parser = new LuaParser(context, this.fileName);

			SetBuilderTable(builder, context);
			SetRefletionTable(builder, context);

			parser.Parse();

			//lua = new Lua();
			//{
			//    lua["builder"] = new AutofacLuaBuilder(builder);
			//    lua.DoString(File.ReadAllText(fileName));
			//}
		}

		private static void SetBuilderTable(ContainerBuilder builder, LuaContext context)
		{
			var b = new AutofacLuaBuilder(builder);
			var builderTable =
				LuaObject.FromTable(
					new Dictionary<LuaObject, LuaObject>()
						{
							{ "As", LuaObject.FromDelegate((Func<object, object, object>)(b.As)) },
							{ "RegisterGeneric", LuaObject.FromDelegate((Func<object, object>)(b.RegisterGeneric)) },
							{ "RegisterType", LuaObject.FromDelegate((Func<object, object>)(b.RegisterType)) },
							{ "RegisterModule", LuaObject.FromDelegate((Action<object>)(b.RegisterModule)) },
							{ "RegisterInstance", LuaObject.FromDelegate((Func<object, object>)(b.RegisterInstance)) },
							{ "Register", LuaObject.FromDelegate((Func<LuaFunction, object>)(b.Register)) },
							{ "InstancePerDependency", LuaObject.FromDelegate((Func<object, object>)(b.InstancePerDependency)) },
							{ "SingleInstance", LuaObject.FromDelegate((Func<object, object>)(b.SingleInstance)) },
						});
			context.SetGlobal("builder", builderTable);
		}
		private static void SetRefletionTable(ContainerBuilder builder, LuaContext context)
		{
			var b = new AutofacLuaReflection();
			var builderTable =
				LuaObject.FromTable(
					new Dictionary<LuaObject, LuaObject>()
						{
							{ "Activate", LuaObject.FromFunction(b.Activate) },
							{ "Resolve", LuaObject.FromDelegate((Func<IComponentContext, object, object>)b.Resolve) },
							{ "Invoke", LuaObject.FromFunction(b.Invoke) },
							{ "GetValue", LuaObject.FromFunction(b.GetValue) },
							{ "SetValue", LuaObject.FromFunction(b.SetValue) },
							{ "Type", LuaObject.FromDelegate((Func<object, object>)b.Type) },
							{ "TypeOf", LuaObject.FromDelegate((Func<object, object>)b.TypeOf) },
						});
			context.SetGlobal("reflection", builderTable);
		}
		#endregion
	}
}