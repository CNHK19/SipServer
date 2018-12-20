// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Globalization;
using System.Windows.Controls;
using Uccapi;

namespace Messenger.Windows
{
	public class GroupValidationRule
		: ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value != null)
				if ((value as string).Length > 31)
					return new ValidationResult(false, "Too long Group.");

			return new ValidationResult(true, null);
		}
	}
}
