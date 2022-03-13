//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using XConfig;

public partial class Config : ConfigBase
{
	public static Config Inst => _inst ?? (_inst = new Config());
	static Config _inst;

	[BindConfigFileName("base_ref_type", false)]
	public BaseRefTypeTable baseRefTypeTable;
	[BindConfigFileName("base_type", false)]
	public BaseTypeTable baseTypeTable;
	[BindConfigFileName("child1", false)]
	public Child1Table child1Table;
	[BindConfigFileName("child2", false)]
	public Child2Table child2Table;
	[BindConfigFileName("custom_config_type", false)]
	public CustomConfigTypeTable customConfigTypeTable;
	[BindConfigFileName("double_key", false)]
	public DoubleKeyTable doubleKeyTable;
	[BindConfigFileName("hello_world", false)]
	public HelloWorldTable helloWorldTable;
	[BindConfigFileName("items", false)]
	public ItemsTable itemsTable;
	[BindConfigFileName("parent", true)]
	public ParentTable parentTable;
	[BindConfigFileName("partial_table", false)]
	public PartialTableTable partialTableTable;

	public override void Init(bool isFromGenerateConfig = false)
	{
		baseRefTypeTable = new BaseRefTypeTable();
		baseTypeTable = new BaseTypeTable();
		child1Table = new Child1Table();
		child2Table = new Child2Table();
		customConfigTypeTable = new CustomConfigTypeTable();
		doubleKeyTable = new DoubleKeyTable();
		helloWorldTable = new HelloWorldTable();
		itemsTable = new ItemsTable();
		parentTable = new ParentTable();
		partialTableTable = new PartialTableTable();

		base.Init(isFromGenerateConfig);
	}
}
