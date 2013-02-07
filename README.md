Autofac.AluminumLua
===================

Autofac configuration by Lua script file.

Examples
========

Register callback function
--------------------------

c#:
```c#
builder.Register(GetMongoClient);
```

lua:
```lua
function GetMongoClient(context)
  return reflection.Activate("MongoDB.Driver.MongoClient, MongoDB.Driver","mongodb://localhost/SchedulerTests")
end
builder.Register(GetMongoClient)
```


Full real live example
----------------------

```lua
function GetMongoClient(context)
  return reflection.Activate("MongoDB.Driver.MongoClient, MongoDB.Driver","mongodb://localhost/SchedulerTests")
end

function GetMongoDatabase(context)
	db = reflection.Resolve(context,"MongoDB.Driver.MongoServer, MongoDB.Driver")
	return reflection.Invoke(db,"GetDatabase","SchedulerTests")
end

function GetMongoServer(context)
	return reflection.Invoke(reflection.Resolve(context,"MongoDB.Driver.MongoClient, MongoDB.Driver"),"GetServer")
end

builder.InstancePerDependency(builder.As(builder.RegisterGeneric("NaiveScheduler.MongoDB.MongoDbSchedulerStorage`1, NaiveScheduler.MongoDB"), "NaiveScheduler.Core.ISchedulerStorage`1, NaiveScheduler.Core"))

builder.SingleInstance(builder.As(builder.RegisterType("NaiveScheduler.Core.DefaultTimeProvider, NaiveScheduler.Core"), "NaiveScheduler.Core.ITimeProvider, NaiveScheduler.Core"))
builder.SingleInstance(builder.As(builder.Register(GetMongoClient), "MongoDB.Driver.MongoClient, MongoDB.Driver"))
builder.InstancePerDependency(builder.As(builder.Register(GetMongoDatabase),"MongoDB.Driver.MongoDatabase, MongoDB.Driver" ))
builder.InstancePerDependency(builder.As(builder.Register(GetMongoServer),"MongoDB.Driver.MongoServer, MongoDB.Driver" ))
```
