// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Drawing;

namespace RichTextBoxEx
{
	class EmoticonDescription
	{
		public string[] Smiles
		{
			get;
			set;
		}

		public Bitmap Emoticon
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string Smilez
		{
			set
			{
				Smiles = value.Split(new char[] { '\t', '\n', '\r', ' ', });
			}
		}
	}

	class YahooEmoticons
	{
		static YahooEmoticons()
		{
			Emoticons = new EmoticonDescription[]
			{
				new EmoticonDescription() { Smilez = @":) :-)", Description="happy", Emoticon = Resources._1, },
				new EmoticonDescription() { Smilez = @":( :-(", Description="sad", Emoticon = Resources._2, },
				new EmoticonDescription() { Smilez = @";) ;-)", Description="winking", Emoticon = Resources._3, },
				new EmoticonDescription() { Smilez = @":D :-D", Description="big grin", Emoticon = Resources._4, },
				new EmoticonDescription() { Smilez = @";;)", Description="batting eyelashes", Emoticon = Resources._5, },
				new EmoticonDescription() { Smilez = @">:D<", Description="big hug", Emoticon = Resources._6, },
				new EmoticonDescription() { Smilez = @":/ :-/", Description="confused", Emoticon = Resources._7, },
				new EmoticonDescription() { Smilez = @":x :-x", Description="love struck", Emoticon = Resources._8, },
				new EmoticonDescription() { Smilez = ":\">", Description="blushing", Emoticon = Resources._9, },
				new EmoticonDescription() { Smilez = @":P :-P", Description="tongue", Emoticon = Resources._10, },
				
				new EmoticonDescription() { Smilez = @":-*", Description="kiss", Emoticon = Resources._11, },
				new EmoticonDescription() { Smilez = @"=((", Description="broken heart", Emoticon = Resources._12, },
				new EmoticonDescription() { Smilez = @":-O", Description="surprise", Emoticon = Resources._13, },
				new EmoticonDescription() { Smilez = @"X(", Description="angry", Emoticon = Resources._14, },
				new EmoticonDescription() { Smilez = @":>", Description="smug", Emoticon = Resources._15, },
				new EmoticonDescription() { Smilez = @"B-)", Description="cool", Emoticon = Resources._16, },
				new EmoticonDescription() { Smilez = @":-S", Description="worried", Emoticon = Resources._17, },
				new EmoticonDescription() { Smilez = @"#:-S", Description="whew!", Emoticon = Resources._18, },
				new EmoticonDescription() { Smilez = @">:)", Description="devil", Emoticon = Resources._19, },
				new EmoticonDescription() { Smilez = @":((", Description="crying", Emoticon = Resources._20, },
				
				new EmoticonDescription() { Smilez = @":))", Description="laughing", Emoticon = Resources._21, },
				new EmoticonDescription() { Smilez = @":| :-|", Description="straight face", Emoticon = Resources._22, },
				new EmoticonDescription() { Smilez = @"/:)", Description="raised eyebrows", Emoticon = Resources._23, },
				new EmoticonDescription() { Smilez = @"=))", Description="rolling on the floor", Emoticon = Resources._24, },
				new EmoticonDescription() { Smilez = @"O:-)", Description="angel", Emoticon = Resources._25, },
				new EmoticonDescription() { Smilez = @":-B", Description="nerd", Emoticon = Resources._26, },
				new EmoticonDescription() { Smilez = @"=;", Description="talk to the hand", Emoticon = Resources._27, },
				new EmoticonDescription() { Smilez = @":-c", Description="call me", Emoticon = Resources._101, },
				new EmoticonDescription() { Smilez = @":)]", Description="on the phone", Emoticon = Resources._100, },
				new EmoticonDescription() { Smilez = @"~X(", Description="at wits' end", Emoticon = Resources._102, },
				
				new EmoticonDescription() { Smilez = @":-h", Description="wave", Emoticon = Resources._103, },
				new EmoticonDescription() { Smilez = @":-t", Description="time out", Emoticon = Resources._104, },
				new EmoticonDescription() { Smilez = @"8->", Description="day dreaming", Emoticon = Resources._105, },
				new EmoticonDescription() { Smilez = @"I-)", Description="sleepy", Emoticon = Resources._28, },
				new EmoticonDescription() { Smilez = @"8-|", Description="rolling eyes", Emoticon = Resources._29, },
				new EmoticonDescription() { Smilez = @"L-)", Description="loser", Emoticon = Resources._30, },
				new EmoticonDescription() { Smilez = @":-&", Description="sick", Emoticon = Resources._31, },
				new EmoticonDescription() { Smilez = @":-$", Description="don't tell anyone", Emoticon = Resources._32, },
				new EmoticonDescription() { Smilez = @"[-(", Description="no talking", Emoticon = Resources._33, },
				new EmoticonDescription() { Smilez = @":O)", Description="clown", Emoticon = Resources._34, },
				
				new EmoticonDescription() { Smilez = @"8-}", Description="silly", Emoticon = Resources._35, },
				new EmoticonDescription() { Smilez = @"<:-P", Description="party", Emoticon = Resources._36, },
				new EmoticonDescription() { Smilez = @"(:|", Description="yawn", Emoticon = Resources._37, },
				new EmoticonDescription() { Smilez = @"=P~", Description="drooling", Emoticon = Resources._38, },
				new EmoticonDescription() { Smilez = @":-?", Description="thinking", Emoticon = Resources._39, },
				new EmoticonDescription() { Smilez = @"#-o", Description="d'oh", Emoticon = Resources._40, },
				new EmoticonDescription() { Smilez = @"=D>", Description="applause", Emoticon = Resources._41, },
				new EmoticonDescription() { Smilez = @":-SS", Description="nail biting", Emoticon = Resources._42, },
				new EmoticonDescription() { Smilez = @"@-)", Description="hypnotized", Emoticon = Resources._43, },
				new EmoticonDescription() { Smilez = @":^o", Description="liar", Emoticon = Resources._44, },
				
				new EmoticonDescription() { Smilez = @":-w", Description="waiting", Emoticon = Resources._45, },
				new EmoticonDescription() { Smilez = @":-<", Description="sigh", Emoticon = Resources._46, },
				new EmoticonDescription() { Smilez = @">:P", Description="phbbbbt", Emoticon = Resources._47, },
				new EmoticonDescription() { Smilez = @"<):)", Description="cowboy", Emoticon = Resources._48, },
				new EmoticonDescription() { Smilez = @"X_X", Description="I don't want to see", Emoticon = Resources._109, },
				new EmoticonDescription() { Smilez = @":!!", Description="hurry up!", Emoticon = Resources._110, },
				new EmoticonDescription() { Smilez = @"\m/", Description="rock on!", Emoticon = Resources._111, },
				new EmoticonDescription() { Smilez = @":-q", Description="thumbs down", Emoticon = Resources._112, },
				new EmoticonDescription() { Smilez = @":-bd", Description="thumbs up", Emoticon = Resources._113, },
				new EmoticonDescription() { Smilez = @"^#(^", Description="it wasn't me", Emoticon = Resources._114, },
				
				new EmoticonDescription() { Smilez = @":ar!", Description="pirate", Emoticon = Resources.pirate_2, },
				new EmoticonDescription() { Smilez = @":o3", Description="puppy dog eyes", Emoticon = Resources._108, },
				new EmoticonDescription() { Smilez = @":-??", Description="I don't know", Emoticon = Resources._106, },
				new EmoticonDescription() { Smilez = @"%-(", Description="not listening", Emoticon = Resources._107, },
				new EmoticonDescription() { Smilez = @":@)", Description="pig", Emoticon = Resources._49, },
				new EmoticonDescription() { Smilez = @"3:-O", Description="cow", Emoticon = Resources._50, },
				new EmoticonDescription() { Smilez = @":(|)", Description="monkey", Emoticon = Resources._51, },
				new EmoticonDescription() { Smilez = @"~:>", Description="chicken", Emoticon = Resources._52, },
				new EmoticonDescription() { Smilez = @"@};-", Description="rose", Emoticon = Resources._53, },
				new EmoticonDescription() { Smilez = @"%%-", Description="good luck", Emoticon = Resources._54, },

				new EmoticonDescription() { Smilez = @"**==", Description="flag", Emoticon = Resources._55, },
				new EmoticonDescription() { Smilez = @"(~~)", Description="pumpkin", Emoticon = Resources._56, },
				new EmoticonDescription() { Smilez = @"~O)", Description="coffee", Emoticon = Resources._57, },
				new EmoticonDescription() { Smilez = @"*-:)", Description="idea", Emoticon = Resources._58, },
				new EmoticonDescription() { Smilez = @"8-X", Description="skull", Emoticon = Resources._59, },
				new EmoticonDescription() { Smilez = @"=:)", Description="bug", Emoticon = Resources._60, },
				new EmoticonDescription() { Smilez = @">-)", Description="alien", Emoticon = Resources._61, },
				new EmoticonDescription() { Smilez = @":-L", Description="frustrated", Emoticon = Resources._62, },
				new EmoticonDescription() { Smilez = @"[-O<", Description="praying", Emoticon = Resources._63, },
				new EmoticonDescription() { Smilez = @"$-)", Description="money eyes", Emoticon = Resources._64, },
	
				new EmoticonDescription() { Smilez = ":-\"", Description="whistling", Emoticon = Resources._65, },
				new EmoticonDescription() { Smilez = @"b-(", Description="feeling beat up", Emoticon = Resources._66, },
				new EmoticonDescription() { Smilez = @":)>-", Description="peace sign", Emoticon = Resources._67, },
				new EmoticonDescription() { Smilez = @"[-X", Description="shame on you", Emoticon = Resources._68, },
				new EmoticonDescription() { Smilez = @"\:D/", Description="dancing", Emoticon = Resources._69, },
				new EmoticonDescription() { Smilez = @">:/", Description="bring it on", Emoticon = Resources._70, },
				new EmoticonDescription() { Smilez = @";))", Description="hee hee", Emoticon = Resources._71, },
				new EmoticonDescription() { Smilez = @":-@", Description="chatterbox", Emoticon = Resources._76, },
				new EmoticonDescription() { Smilez = @"^:)^", Description="not worthy", Emoticon = Resources._77, },
				new EmoticonDescription() { Smilez = @":-j", Description="oh go on", Emoticon = Resources._78, },
				
				new EmoticonDescription() { Smilez = @"(*)", Description="star", Emoticon = Resources._79, },
				new EmoticonDescription() { Smilez = @"o->", Description="hiro", Emoticon = Resources._72, },
				new EmoticonDescription() { Smilez = @"o=>", Description="billy", Emoticon = Resources._73, },
				new EmoticonDescription() { Smilez = @"o-+", Description="april", Emoticon = Resources._74, },
				new EmoticonDescription() { Smilez = @"(%)", Description="yin yang", Emoticon = Resources._75, },
				new EmoticonDescription() { Smilez = @":bz", Description="bee", Emoticon = Resources._115, },
//				new EmoticonDescription() { Smilez = @"[..]", Description="transformer", Emoticon = Resources.transformer, },
			};
		}

		public static EmoticonDescription[] Emoticons
		{
			get;
			private set;
		}
	}
}
