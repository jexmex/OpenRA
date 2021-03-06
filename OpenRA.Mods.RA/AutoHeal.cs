﻿#region Copyright & License Information
/*
 * Copyright 2007-2011 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using System.Linq;
using OpenRA.Traits;

namespace OpenRA.Mods.RA
{
	class AutoHealInfo : TraitInfo<AutoHeal>, Requires<AttackBaseInfo> { }

	class AutoHeal : INotifyIdle
	{
		public void TickIdle( Actor self )
		{
			var attack = self.Trait<AttackBase>();
			var inRange = self.World.FindUnitsInCircle(self.CenterLocation, (int)(Game.CellSize * attack.GetMaximumRange()));

			var target = inRange
				.Where(a => a != self && a.AppearsFriendlyTo(self))
				.Where(a => a.IsInWorld && !a.IsDead())
				.Where(a => a.GetDamageState() > DamageState.Undamaged)
				.Where(a => attack.HasAnyValidWeapons(Target.FromActor(a)))
				.ClosestTo( self.CenterLocation );

			if( target != null )
				self.QueueActivity(attack.GetAttackActivity(self, Target.FromActor( target ), false ));
		}
	}
}
