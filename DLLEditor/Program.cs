using ADV;
using ModdingCore;
using ModUtils;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

internal class Program
{
    static string inputDirectory = "\"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Holder of Place\\HolderOfPlace_Data\\Managed";
    static string inputPath = "C:\\Users\\Michael\\Desktop\\HolderOfPlace\\Assembly-CSharp.dll";
    static string modPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Holder of Place\\HolderOfPlace_Data\\Managed\\ModBootstrap.dll";

    static string outputPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Holder of Place\\HolderOfPlace_Data\\Managed\\Assembly-CSharp.dll";
    static void Main(string[] args)
    {
        var assemblyPath = Path.GetFullPath(inputPath);


        var resolver = new DefaultAssemblyResolver();
        resolver.AddSearchDirectory(inputDirectory);

        AssemblyDefinition _assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters { AssemblyResolver = resolver });
        AssemblyDefinition _mod = AssemblyDefinition.ReadAssembly( modPath, new ReaderParameters { AssemblyResolver = resolver });

        Console.WriteLine("Assemblies Found!");

        //2. MAIN CODE BLOCK!!!
        var _titleScreen = FindMethod(_assembly, nameof(TitleScreenControl), "Start");
        var processor = _titleScreen.Body.GetILProcessor();
        var instruction = processor.Body.Instructions[0];
        processor.InsertBefore(instruction, processor.Create(OpCodes.Call, GetMethodReference<BootstrapMain>(_assembly, nameof(BootstrapMain.Start), Array.Empty<Type>())));
        Console.WriteLine("1st Change Successful!");

        //Event Insertion: Card Generated
        //Spot 1: RecruitPanel.Generate
        //Spot 2: EncounterPanel.NewCards
        //Spot 3: EncounterPanel.ImportAdditionalCards
        if (
            AddCardGeneratedEvent(_assembly, nameof(RecruitPanel), nameof(RecruitPanel.Generate), 5, OpCodes.Ldloc_0, null) &&
            AddCardGeneratedEvent(_assembly, nameof(EncounterPanel), nameof(EncounterPanel.NewCards), 1, OpCodes.Ldloc, 8) &&
            AddCardGeneratedEvent(_assembly, nameof(EncounterPanel), nameof(EncounterPanel.ImportAdditionalCards), 0, OpCodes.Ldloc, 8)
            )
        {
            Console.WriteLine("3rd Change Successful!");
        }
        //4+5. Button changes
        var _buttonUpdate = FindMethod(_assembly, nameof(UIButton), nameof(UIButton.Update), 0);
        processor = _buttonUpdate.Body.GetILProcessor();
        for(int i=0; i<processor.Body.Instructions.Count; i++)
        {
            var ins = processor.Body.Instructions[i];
            //System.Console.WriteLine(ins.Operand?.ToString() ?? "null");
            if (ins.Operand?.ToString() == "System.Void ADV.UIButton::MouseDownEffect()")
            {
                var postIns = processor.Body.Instructions[i+1];
                var endOfCheck = processor.Body.Instructions[i - 2];
                processor.InsertAfter(endOfCheck, processor.Create(OpCodes.Brfalse_S, postIns));
                processor.InsertAfter(endOfCheck, processor.Create(OpCodes.Call, GetMethodReference<BootstrapMain>(_assembly, nameof(BootstrapMain.IsInputAllowed), Array.Empty<Type>())));
                Console.WriteLine("4th Change Successful!");
                i += 2;
            }
            if (ins.Operand?.ToString() == "System.Void ADV.UIButton::MouseUpEffect()")
            {
                var postIns = processor.Body.Instructions[i + 1];
                var endOfCheck = processor.Body.Instructions[i - 2];
                processor.InsertBefore(endOfCheck, processor.Create(OpCodes.Brfalse, postIns));
                processor.InsertBefore(endOfCheck, processor.Create(OpCodes.Call, GetMethodReference<BootstrapMain>(_assembly, nameof(BootstrapMain.IsInputAllowed), Array.Empty<Type>())));
                Console.WriteLine("5th Change Successful!");
                i += 2;
            }
        }


        Console.WriteLine("Outputting!");
        _assembly.Write(outputPath);
        Console.WriteLine("Done!");
    }

    static bool AddCardGeneratedEvent(AssemblyDefinition _assembly, string className, string methodName, int parameterCount, OpCode op, short? obj)
    {
        var _cardGenerated = FindMethod(_assembly, className, methodName, parameterCount);
        var processor = _cardGenerated.Body.GetILProcessor();
        var instruction = processor.Body.Instructions.FirstOrDefault(i =>
        {
            return (i.Operand?.ToString() == "!!0 UnityEngine.Object::Instantiate<UnityEngine.GameObject>(!!0,UnityEngine.Transform)");
        });
        if (instruction == null)
        {
            Console.WriteLine("Change Unsuccessful ("+className +"."+methodName+")");
            return false;
        }
        int index = processor.Body.Instructions.ToList().IndexOf(instruction) + 2;
        object operand = processor.Body.Instructions[index].Operand;
        processor.InsertAfter(index, processor.Create(OpCodes.Call, GetMethodReference<ModEvents>(_assembly, nameof(ModEvents.InvokeCardGenerated), new Type[] { typeof(Card) })));
        if (obj == null)
        {
            processor.InsertAfter(index, processor.Create(op));
        }
        else
        {
            System.Console.WriteLine(op.OperandType);
            processor.InsertAfter(index, processor.Create(op,(short)obj));
        }
        return true;
    }

    static PropertyDefinition FindProperty(AssemblyDefinition _assembly, string type, string property)
    {
        var _type = _assembly.MainModule.Types.First(t => t.Name == type);
        var _property = _type.Properties.FirstOrDefault(f => f.Name == property);
        Console.WriteLine($"Found Method: {type}.{property}");
        return _property;
    }

    static MethodReference GetMethodReference<T>(AssemblyDefinition _assembly, string methodName, Type[] args)
    {
        MethodInfo info = typeof(T).GetMethod(methodName, args);
        MethodReference reference = _assembly.MainModule.ImportReference(info);
        return reference;
    }

    static MethodDefinition FindMethod(AssemblyDefinition _assembly, string type, string method, int parameters = 0)
    {
        Console.WriteLine($"Searching for Method: {type}.{method}");
        var _type = _assembly.MainModule.Types.FirstOrDefault(t => t.Name == type);
        var _method = _type.Methods.FirstOrDefault(_m => _m.Name == method && _m.Parameters.Count == parameters);
        Console.WriteLine($"Found Method: {type}.{method}");
        return _method;
    }

    static MethodDefinition FindStaticMethod(AssemblyDefinition _assembly, string type, string method, int parameters = 0)
    {
        var _type = _assembly.MainModule.Types.FirstOrDefault(t => t.Name == type);
        var _method = _type.Methods.FirstOrDefault(_m => _m.Name == method && _m.IsStatic && _m.Parameters.Count <= parameters);
        Console.WriteLine($"Found Method: {type}.{method}");
        return _method;
    }
}