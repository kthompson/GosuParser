# GosuParser [![NuGet Version](http://img.shields.io/nuget/v/GosuParser.svg?style=flat)](https://www.nuget.org/packages/GosuParser/) 

A parser combinator library for .net

# Example

```c#
using static GosuParser.Parser;

public static Parser<object> GetJsonParser()
{
  Action<Parser<object>> assignment;
  Parser<object> jvalue = CreateParserForwardedToRef(out assignment);

  var jstring = from chars in Satisfy(c => c != '"' && c != '\\').Many().Between(Char('"'), Char('"'))
                select new string(chars.ToArray());
  var key = jstring.WithLabel("key");
  jstring = jstring.WithLabel("string");
  var keyPair = key.TakeLeft(String(":")).AndThen(jvalue).WithLabel("KeyPair");
  var keyPairs = keyPair.SepBy(String(","));
  var jobject = from pairs in keyPairs.Between(Char('{'), Char('}'))
                select (object)pairs.ToDictionary(t => t.Item1, t => t.Item2);

  var jarray = from value in jvalue.SepBy(Char(',')).Between(Char('['), Char(']'))
                select (object)value.ToArray();

  var jnumber = from i in IntParser()
                select (object)i;
  assignment(new[]
  {
      jstring.Select(x => (object) x),
      jnumber,
      jobject,
      jarray,
      String("true").Select(x => (object) true),
      String("false").Select(x => (object) false),
      String("null").Select(x => (object) null)
  }.Choice().WithLabel("value"));

  return jvalue;
}
```
