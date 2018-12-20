// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Globalization;
using System.Windows.Controls;

namespace Messenger.Windows
{
	public class UriValidationRule
		: ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (Uccapi.Helpers.IsInvalidUri(Uccapi.Helpers.CorrectUri(value as string)))
				return new ValidationResult(false, "Uri is invalid, example: user@domain.zone");

			return new ValidationResult(true, null);
		}
	}
}
