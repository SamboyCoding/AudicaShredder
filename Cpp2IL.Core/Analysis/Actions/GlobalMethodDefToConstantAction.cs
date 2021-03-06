﻿using System;
using System.Linq;
using Cpp2IL.Core.Analysis.ResultModels;
using Iced.Intel;
using LibCpp2IL;
using LibCpp2IL.Metadata;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Instruction = Iced.Intel.Instruction;

namespace Cpp2IL.Core.Analysis.Actions
{
    public class GlobalMethodDefToConstantAction : BaseAction
    {
        public Il2CppMethodDefinition? MethodData;
        public MethodDefinition? ResolvedMethod;
        public ConstantDefinition? ConstantWritten;
        
        public GlobalMethodDefToConstantAction(MethodAnalysis context, Instruction instruction) : base(context, instruction)
        {
            var globalAddress = LibCpp2IlMain.Binary.is32Bit ? instruction.MemoryDisplacement64 : instruction.GetRipBasedInstructionMemoryAddress();
            MethodData = LibCpp2IlMain.GetMethodDefinitionByGlobalAddress(globalAddress);
            var type = SharedState.UnmanagedToManagedTypes[MethodData!.DeclaringType];

            if (type == null)
            {
                Console.WriteLine("Failed to lookup managed type for declaring type of " + MethodData.GlobalKey + ", which is " + MethodData.DeclaringType.FullName);
                return;
            }
            
            ResolvedMethod = type.Methods.FirstOrDefault(m => m.Name == MethodData.Name);

            if (ResolvedMethod == null) return;
            
            var destReg = instruction.Op0Kind == OpKind.Register ? Utils.GetRegisterNameNew(instruction.Op0Register) : null;
            var name = ResolvedMethod.Name;
            
            ConstantWritten = context.MakeConstant(typeof(MethodReference), ResolvedMethod, name, destReg);
        }

        public override Mono.Cecil.Cil.Instruction[] ToILInstructions(MethodAnalysis context, ILProcessor processor)
        {
            throw new System.NotImplementedException();
        }

        public override string ToPsuedoCode()
        {
            throw new System.NotImplementedException();
        }

        public override string ToTextSummary()
        {
            return $"Loads the method definition for managed method {ResolvedMethod!.FullName} as a constant \"{ConstantWritten?.Name}\"";
        }
    }
}