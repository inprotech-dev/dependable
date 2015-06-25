﻿using System;
using System.Threading.Tasks;

namespace Dependable.Core
{
    public class UnitAtom : Atom<Unit>
    {
        public Func<Task> Action { get; }

        public UnitAtom(Action action)
        {
            Action = () =>
            {
                action();
                return Unit.CompletedTask;
            };
        }

        public UnitAtom(Func<Task> action)
        {
            Action = action;
        }

        public override async Task<Unit> Charge(object input = null)
        {
            await Action();
            return Unit.Value;
        }
    }

    public static partial class Atom
    {
        public static UnitAtom Of(Action impl)
        {
            return new UnitAtom(impl);
        }

        public static UnitAtom Of(Func<Task> impl)
        {
            return new UnitAtom(impl);
        }
    }
}