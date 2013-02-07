Autofac.AluminumLua
===================

Autofac configuration by Lua script file.

Examples
========

Register callback function
--------------------------

c#:
```c#
builder.Register(Fn);
```

lua:
```lua
function GetMongoClient(context)
  return builder.Activate("MongoDB.Driver.MongoClient, MongoDB.Driver","mongodb://localhost/SchedulerTests")
end
builder.Register(GetMongoClient)
```
