//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: InputFile/cs_login.proto
namespace cs
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CSLoginInfo")]
  public partial class CSLoginInfo : global::ProtoBuf.IExtensible
  {
    public CSLoginInfo() {}
    
    private string _UserName;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"UserName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string UserName
    {
      get { return _UserName; }
      set { _UserName = value; }
    }
    private string _Password;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"Password", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string Password
    {
      get { return _Password; }
      set { _Password = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CSLoginReq")]
  public partial class CSLoginReq : global::ProtoBuf.IExtensible
  {
    public CSLoginReq() {}
    
    private cs.CSLoginInfo _LoginInfo;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"LoginInfo", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public cs.CSLoginInfo LoginInfo
    {
      get { return _LoginInfo; }
      set { _LoginInfo = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CSLoginRes")]
  public partial class CSLoginRes : global::ProtoBuf.IExtensible
  {
    public CSLoginRes() {}
    
    private uint _result_code;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"result_code", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint result_code
    {
      get { return _result_code; }
      set { _result_code = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}