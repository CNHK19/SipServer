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
	public class NotEmptyValidationRule
		: ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			bool result = false;

			if (value is string)
				result = !string.IsNullOrEmpty(value as string);
			else
				result = value != null;

			return new ValidationResult(result, @"Empty value is not allowed.");
		}
	}
}
