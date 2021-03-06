﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM
{
    public class State0 : State
    {
        internal readonly IDictionary<Def.Event.ISingleAction, IEnumerable<IfButtonGestureDefinition>> T0;
        internal readonly IDictionary<Def.Event.IDoubleActionSet, IEnumerable<OnButtonGestureDefinition>> T1;
        internal readonly IDictionary<Def.Event.IDoubleActionSet, IEnumerable<IfButtonGestureDefinition>> T2;

        public State0(
            StateGlobal Global,
            IEnumerable<GestureDefinition> gestureDef)
            : base(Global)
        {
            this.T0 = Transition.Gen0_0(gestureDef);
            this.T1 = Transition.Gen0_1(gestureDef);
            this.T2 = Transition.Gen1_3(gestureDef);
        }

        public override Result Input(Def.Event.IEvent evnt, Point point)
        {
            // Special side effect 3, 4
            if (MustBeIgnored(evnt))
            {
                return Result.EventIsConsumed(nextState: this);
            }

            if (evnt is Def.Event.ISingleAction)
            {
                var ev = evnt as Def.Event.ISingleAction;
                if (T0.Keys.Contains(ev))
                {
                    var ctx = new UserActionExecutionContext(point);
                    var _T0 = FilterByWhenClause(ctx, T0[ev]);
                    if (_T0.Count() > 0)
                    {
                        Debug.Print("[Transition 0_0]");
                        ExecuteUserDoFuncInBackground(ctx, _T0);
                        return Result.EventIsConsumed(nextState: this);
                    }
                }
            }
            else if (evnt is Def.Event.IDoubleActionSet)
            {
                var ev = evnt as Def.Event.IDoubleActionSet;
                if (T1.Keys.Contains(ev) || T2.Keys.Contains(ev))
                {
                    var ctx = new UserActionExecutionContext(point);
                    var cache = new Dictionary<DSL.Def.WhenFunc, bool>();
                    var _T1 = T1.Keys.Contains(ev) ? FilterByWhenClause(ctx, T1[ev], cache) : new List<OnButtonGestureDefinition>();
                    var _T2 = T2.Keys.Contains(ev) ? FilterByWhenClause(ctx, T2[ev], cache) : new List<IfButtonGestureDefinition>();
                    if (_T1.Count() > 0 || _T2.Count() > 0)
                    {
                        Debug.Print("[Transition 0_1]");
                        ExecuteUserBeforeFuncInBackground(ctx, _T2);
                        return Result.EventIsConsumed(nextState: new State1(Global, this, ctx, ev, _T1, _T2));
                    }
                }
            }
            return base.Input(evnt, point);
        }
        
        public override IState Reset()
        {
            Debug.Print("[Transition 0_2]");
            return this;
        }

        internal static IEnumerable<T> FilterByWhenClause<T>(
            UserActionExecutionContext ctx,
            IEnumerable<T> gestureDef) 
            where T : IWhenEvaluatable
        {
            return FilterByWhenClause(ctx, gestureDef, new Dictionary<DSL.Def.WhenFunc, bool>());
        }

        internal static IEnumerable<T> FilterByWhenClause<T>(
            UserActionExecutionContext ctx,
            IEnumerable<T> gestureDef,
            Dictionary<DSL.Def.WhenFunc, bool> cache) 
            where T : IWhenEvaluatable
        {
            // This evaluation of functions given as the parameter of `@when` clause can be executed in parallel, 
            // but executing it in sequential order here for simplicity.
            return gestureDef
                .Where(x => x.EvaluateUserWhenFunc(ctx, cache))
                .ToList();
        }
    }
}
