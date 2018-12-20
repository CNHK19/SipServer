// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Messenger.Windows
{
	class PropertiesBinding
	{
		public delegate object Getter();
		public delegate void Setter(object value);

		public Getter SourceGetter { get; set; }
		public Setter SourceSetter { get; set; }
		public Getter TargetGetter { get; set; }
		public Setter TargetSetter { get; set; }

		public static void CopyToTarget(PropertiesBinding[] propertiesBindings)
		{
			Array.ForEach<PropertiesBinding>(propertiesBindings,
				(binding) => { binding.TargetSetter(binding.SourceGetter()); });
		}

		public static void CopyToSource(PropertiesBinding[] propertiesBindings)
		{
			Array.ForEach<PropertiesBinding>(propertiesBindings,
				(binding) => { binding.SourceSetter(binding.TargetGetter()); });
		}

		public static bool SourceEqualsTarget(PropertiesBinding[] propertiesBindings)
		{
			return Array.TrueForAll(propertiesBindings,
				(binding) =>
				{
					var sourceValue = binding.SourceGetter();
					if (sourceValue is bool || sourceValue is int)
						return sourceValue.Equals(binding.TargetGetter());
					if (sourceValue is string)
						return (string)sourceValue == (string)binding.TargetGetter();
					throw new NotImplementedException();
				});
		}

		public static bool SourceNotEqualsTarget(PropertiesBinding[] propertiesBindings)
		{
			return !SourceEqualsTarget(propertiesBindings);
		}
	}
}
