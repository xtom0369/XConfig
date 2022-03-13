## ����

XConfig ��һ������Unity���桢֧��**csv => bytes**�����������ñ�ϵͳ���ṩ�����µ����ԣ�

- **�Զ����ɱ��������**��һ���������ñ�������룬������ߣ�������룻
- **������������**��֧��byte��bool��int��long��float��Vector2��Color��List�Ȼ����������ͣ�
- **˫����**��֧��������������������Ӣ�۵ȼ�����ҪӢ��id��Ӣ��level����������ͬ���������У�
- **�����**��֧�ֲ�ͬ���ñ�֮����й�����
- **��̳�**��֧�ֽ�����̳й�ϵ��������߷ֶ������͵ĵ��ߣ�ͨ�õ����ֶηŸ�����ͬ���ߵ������ֶη��ӱ�
- **�Զ�������**��֧�ֿ�������Զ����������ͣ�֧�ֺ��Զ�֧��List�ķ�������
- **��/�еĴ�������չ**��֧������չ���жԱ�/�е��ֶ����ݽ��ж��δ������֯
- **��ˢ��**��֧������ʱˢ�����ñ�����ǰֻ֧�ֶ������ֶν����޸ģ���֧�������л��У�����Ӱ������ʱ�����õ����ݣ�

## �ĵ�

- XConfig�̳�

- XConfig����


## ��������

1. ʹ�������аѲֿ����ص����أ�����Unity�򿪹���
```sh
git clone https://github.com/xtom0369/XConfig.git
```

2. **���hello_world.byte���ñ�**����Assets\Example\Config\csv_template.bytes�ļ�������������Ϊhello_world.bytes

3. **�������ñ����**��ִ��Unity�˵���**XConfig/Generate Code**��������Assets\Example\Generate\Code�ļ���������**HelloWorldTable**�����ࣩ��**HelloWorldRow**�����ࣩ���޸ı�ͷ�������ֶ���/�ֶ����ͣ�����Ҫ���µ�������

4. **����Ϊ�������ļ�**��ִ��Unity�˵���**XConfig/Generate Binary**��������Assets\Example\Generate\Bin��������Ӧ��**hello_world.bytes**�������ļ���ÿ���޸����ñ����ݺ���Ҫ���µ�����

5. ��ʼ�����ñ�ģ�飬ֻ��Ҫִ��1��

```CSharp
Config.Inst.Init();
```

6. ��ȡhello_world���ñ�ʵ��

```CSharp
Config.Inst.helloWorldTable;
```

7. ��ȡ���ڵ�����������

```CSharp
Config.Inst.helloWorldTable.rows;
```

8. ��ȡ���ڵ�ĳһ�����ݣ�����������

```CSharp
Config.Inst.helloWorldTable.GetRow();
```

## �ļ��нṹ

    |-- Example
        |-- Config�����ñ��������ӷ��ļ���
        |-- Generate����������
            |-- Bin�����ɵĶ������ļ�
            |-- Code�����ɵĴ���
        |-- Resources���������
        |-- Script���ű����������ӷ��ļ���

    |-- StreamingAssets���������ñ�����assetbundle

    |-- XConfig
        |-- Editor����ܱ༭������
        |-- Runtime���������ʱ����

## ����ʾ��

- [BaseType](https://github.com/xtom0369/XConfig/blob/main/Assets/Example/Script/BaseType.cs)������֧�ֵĻ�����������

- [DoubleKey](https://github.com/xtom0369/XConfig/blob/main/Assets/Example/Script/DoubleKey.cs)��˫��������

- [ParentChildren](https://github.com/xtom0369/XConfig/blob/main/Assets/Example/Script/ParentChildren.cs)����̳�����

- [CustomConfigType](https://github.com/xtom0369/XConfig/blob/main/Assets/Example/Script/CustomConfigType.cs)���Զ�����������

- [PartialTable](https://github.com/xtom0369/XConfig/blob/main/Assets/Example/Script/PartialTable.cs)����/�еĴ�������չ����

- [CustomLoader](https://github.com/xtom0369/XConfig/blob/main/Assets/Example/Script/PartialTable.cs)���Զ���������ӣ�����ab���Զ������

## ����

- QȺ��975919763����֤��Ϣ��дhttps://github.com/xtom0369/XConfig��