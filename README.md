# ArgHelper
ArgHelper provides useful functionalities to parse input ```stirng[] args```.

Input args should be an array of key-value pair: e.g. ```-key1 value1 -key2 value2```.
Supported types of value are
- ```string```
- ```double```
- ```float```
- ```int```
- ```enum```
- ```DateTime```
  - yyyyMMdd
  - yyMMdd
  - yyyy/MM/dd
  - yyyy-MM-dd

## Demo
### How to use
1. Define a class to express the input args.
```
public class SampleArg
{
      [ArgAttribute(key: "path", isMandatory: true, description: "sample path")]
      public string Path { get; set; }
      [ArgAttribute(key: "date", isMandatory: true, description: "sample date")]
      public DateTime Date { get; set; }
      [ArgAttribute(key: "doubleValue", isMandatory: true, description: "sample double value")]
      public double DoubleValue { get; set; }
      [ArgAttribute(key: "enumValue", isMandatory: true, description: "sample enum value")]
      public ESampleEnum EnumValue { get; set; }
      [ArgAttribute(key: "optionalArg", isMandatory: false, description: "sample optional arg value")]
      public string OptionalArg { get; set; }
      
}

public enum ESampleEnum
{
      Enum1,
      Enum2,
}
```

2. Parse input args calling ```Arg.Build<T>(string[] args)```.
```
void Main(string[] args)
{
      //e.g. args = new[] { "-path", @"D:/arghelper/demo/...", "-date", "20200101", "-doubleValue", "1.0", "-enumValue", "Enum1" };

      SampleArg arg = Arg.Build<SampleArg>(args);
      
      string path = arg.Path;         //path = @"D:/arghelper/demo/..."
      DateTime date = arg.Date;       //date = new DateTime(2020, 01, 01)
      double value = arg.DoubleValue; //d = 1.0
      ESampleEnum e = arg.EnumValue;  //e = Enum1
      //arg.OptionalArg is an optional and we don't set the value in this example.
}
```

### Useful function: display descriptions
You can display descriptions of the class which has properties with ArgAttribute attribution with calling ```Arg.DisplayHelpDescription(Type type)``` where type should have properties with ArgAttribute.
```
void Main(string[] args)
{
      //e.g. args = new[] { "-path", @"D:/arghelper/demo/...", "-date", "20200101", "-doubleValue", "1.0", "-enumValue", "Enum1" }
      /*
      +++++ user guide start +++++
        args = -path pathValue -date dateValue -doubleValue doubleValueValue -enumValue enumValueValue
        - mandatory
          -path        : System.String. sample path
          -date        : System.DateTime. sample date
          -doubleValue : System.Double. sample double value
          -enumValue   : Test.ArgHelper.ArgHelperTest+ESampleEnum. sample enum value
        - optional
          -optionalArg : System.String. sample optional arg value
      +++++ user guide end +++++++
      */
}
```

### Useful function: IsHelp
```Arg.IsHelp((string[] args))``` function returns the input args include either ```-help``` or ```help``` or ```-h``` or ```h``` so that you can call, for example, ```Arg.DisplayHelpDescription(Type type)```.
