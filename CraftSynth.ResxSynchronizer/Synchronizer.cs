using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace CraftSynth.ResxSynchronizer
{
	public class ResxItem
	{
		public string Key { get; set; }
		public string Value { get; set; }
		public string Comment { get; set; }
		public DateTime? LastSynchronizationDate { get; set; }
		public DateTime? LastModificationDate { get; set; }
		public bool Changed { get; set; }
		public bool Created { get; set; }
		public bool Deleted { get; set; }

		public override string ToString()
		{
			string r = string.Format("Key:{0}\r\nValue:{1}\r\nComment:{2}\r\nLastSynchronizationDate:{3}\r\nLastModificationDate:{4}\r\n",
				Key,
				Value,
				Comment,
				LastSynchronizationDate==null?"null":LastSynchronizationDate.ToString(),
				LastModificationDate==null?"null":LastModificationDate.ToString());
			return r;
		}

		public void DeleteNode(ref XmlDocument xmlDoc, string key)
		{
			var child = xmlDoc.SelectSingleNode("/root/data[@name='" + key + "']");
			var parent = xmlDoc.SelectSingleNode("/root");
			if (child != null)
			{
				parent.RemoveChild(child);
			}
		}

		public void CopyToNode(ref XmlDocument xmlDoc, ref XmlNode node)
		{
			if (node == null)
			{
				node = xmlDoc.CreateElement("data");
				var attrName = xmlDoc.CreateAttribute("name");
				attrName.InnerText = this.Key;
				node.Attributes.Append(attrName);
				var attrXmlSpace = xmlDoc.CreateAttribute("xml:space");
				attrXmlSpace.InnerText = "preserve";
				node.Attributes.Append(attrXmlSpace);

				var parent = xmlDoc.SelectSingleNode("/root");
				parent.AppendChild(node);
			}

			XmlNode valueNode;
			if (node["value"] == null)
			{
				valueNode = xmlDoc.CreateElement("value");
				node.AppendChild(valueNode);
			}
			else
			{
				valueNode = node["value"];
			}
			valueNode.InnerText = this.Value;
			
			XmlNode commentNode;
			if(node["comment"]==null)
			{
				commentNode = xmlDoc.CreateElement("comment");
				node.AppendChild(commentNode);
			}else
			{
				commentNode = node["comment"];
			}
			commentNode.InnerText = this.Comment;
		}

		public void MoveTimestampsFromCommentToProperties()
		{
			if (this.Comment.IsNOTNullOrWhiteSpace())
			{
				string timestamps = this.Comment.GetSubstring("[", "]");
				if (timestamps.IsNOTNullOrWhiteSpace())
				{
					//example LastSync=GMT2005.12.14 13:15:50,LastChange=GMT2005.10.11 03:33:34
					string patern = @"^LastSync=GMT(?<year>\d{4}).(?<month>\d{2}).(?<day>\d{2}) (?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2}),LastChange=GMT(?<year2>\d{4}).(?<month2>\d{2}).(?<day2>\d{2}) (?<hour2>\d{2}):(?<minute2>\d{2}):(?<second2>\d{2})";
					Regex regex = new Regex(patern, RegexOptions.IgnoreCase);
					Match match = regex.Match(timestamps);
					if (match.Success)
					{
						this.LastSynchronizationDate = new DateTime(
							int.Parse(match.Groups["year"].Value),
							int.Parse(match.Groups["month"].Value),
							int.Parse(match.Groups["day"].Value),
							int.Parse(match.Groups["hour"].Value),
							int.Parse(match.Groups["minute"].Value),
							int.Parse(match.Groups["second"].Value)
							);

						this.LastModificationDate = new DateTime(
							int.Parse(match.Groups["year2"].Value),
							int.Parse(match.Groups["month2"].Value),
							int.Parse(match.Groups["day2"].Value),
							int.Parse(match.Groups["hour2"].Value),
							int.Parse(match.Groups["minute2"].Value),
							int.Parse(match.Groups["second2"].Value)
							);

						this.Comment = this.Comment.RemoveSubstring("[", "]").TrimStart('[').TrimStart(']').TrimStart(' ');
					}
				}
			}
		}

		/// <summary>
		/// //example LastSync=GMT2005.12.14 13:15:50,LastChange=GMT2005.10.11 03:33:34
		/// </summary>
		/// <returns></returns>
		public void ReapplyDatesToComment()
		{
			if(this.Comment==null)
			{
				this.Comment = string.Empty;
			}
			if(this.Comment.StartsWith("[LastSync=") && this.Comment.IndexOf(']')==-1)
			{
				this.Comment = string.Empty;//!!!
			}
			if (this.Comment.Length>0 && this.Comment.StartsWith("[LastSync="))
			{
				this.Comment = this.Comment.Remove(0, this.Comment.IndexOf(']')+1);
			}
			string dates = string.Format("[LastSync=GMT{0},LastChange=GMT{1}] ", this.LastSynchronizationDate.Value.ToString("yyyy.MM.dd HH:mm:ss"), this.LastModificationDate.Value.ToString("yyyy.MM.dd HH:mm:ss"));
			this.Comment = dates + this.Comment;
		}
	}

	public class Synchronizer
	{
		public static bool SynchronizeResxFiles(string resxFilePath1, string resxFilePath2, ref CustomTraceLog customTraceLog, DateTime now, out bool hasConflicts)
		{
			customTraceLog.AddLine(string.Format("Synchronizing {0} and {1}:", Path.GetFileName(resxFilePath1), Path.GetFileName(resxFilePath2)).PadRight(100,'-'));
			customTraceLog.IncreaseIdent();
			
			bool hasErrors = false;
			hasConflicts = false;

			XmlDocument sourceXmlDoc = new XmlDocument();
			sourceXmlDoc.Load(resxFilePath1);

			XmlDocument destinationXmlDoc = new XmlDocument();
			destinationXmlDoc.Load(resxFilePath2);

			bool leftFileChanged = false; 
			bool rightFileChanged = false;

			List<string> processedKeys = new List<string>();
			SynchronizeResxFilesInner(customTraceLog, ref sourceXmlDoc, ref destinationXmlDoc, now, ref hasErrors, ref hasConflicts, ref leftFileChanged, ref rightFileChanged, ref processedKeys);
			SynchronizeResxFilesInner(customTraceLog, ref destinationXmlDoc, ref sourceXmlDoc, now, ref hasErrors, ref hasConflicts, ref leftFileChanged, ref rightFileChanged, ref processedKeys);

			if (leftFileChanged)
			{
				sourceXmlDoc.Save(resxFilePath1);
			}
			if (rightFileChanged)
			{
				destinationXmlDoc.Save(resxFilePath2);
			}

			customTraceLog.DecreaseIdent();
			customTraceLog.AddLine(string.Format("Finished synchronizing {0} and {1}. Has errors:{2}, Has conflicts:{3}", Path.GetFileName(resxFilePath1), Path.GetFileName(resxFilePath2), hasErrors, hasConflicts).PadRight(100, '-'));

			return hasErrors;
		}

		private static void SynchronizeResxFilesInner(CustomTraceLog customTraceLog, ref XmlDocument sourceXmlDoc, ref XmlDocument destinationXmlDoc, DateTime now, ref bool hasErrors, ref bool hasConflicts, ref bool leftFileChanged, ref bool rightFileChanged, ref List<string> processedKeys)
		{
			XmlNodeList nodes = sourceXmlDoc.SelectNodes("/root/data");
			for (int n = nodes.Count - 1;n>=0 ; n--)
			{
				XmlNode node = nodes[n];

				ResxItem sourceResxItem = new ResxItem();
				sourceResxItem.Key = node.Attributes["name"].Value;
				sourceResxItem.Value = node["value"].InnerText;
				sourceResxItem.Comment = node["comment"] == null ? null : node["comment"].InnerText;
				sourceResxItem.MoveTimestampsFromCommentToProperties();//sourceResxItem.Comment!=null && sourceResxItem.Comment.Contains("min pass size")

				if (!processedKeys.Contains(sourceResxItem.Key))
				{
					processedKeys.Add(sourceResxItem.Key);

					ResxItem destinationResxItem = null;
					XmlNode pairInDestinationXml =
						destinationXmlDoc.SelectSingleNode("/root/data[@name='" + node.Attributes["name"].Value + "']");
					if (pairInDestinationXml != null)
					{
						destinationResxItem = new ResxItem();
						destinationResxItem.Key = pairInDestinationXml.Attributes["name"].Value;
						destinationResxItem.Value = pairInDestinationXml["value"].InnerText;
						destinationResxItem.Comment = pairInDestinationXml["comment"] == null
														? null
														: pairInDestinationXml["comment"].InnerText;
						destinationResxItem.MoveTimestampsFromCommentToProperties();
					}

					bool hasC;
					CustomTraceLog tLog = new CustomTraceLog(string.Empty);
					bool hasE = SynchronizeResxItems(ref sourceResxItem, ref destinationResxItem, tLog, now, out hasC);
					hasErrors = hasErrors || hasE;
					hasConflicts = hasConflicts || hasC;

					if (sourceResxItem.Changed || destinationResxItem.Changed || hasE || hasC)
					{
						customTraceLog.ExtendLastLine(node.Attributes["name"].Value.PadRight(80, ' ') + ": ");
						customTraceLog.ExtendLastLine(tLog.ToString());
					}
					if (sourceResxItem.Changed)
					{
						if (sourceResxItem.Deleted)
						{
							sourceResxItem.DeleteNode(ref sourceXmlDoc, sourceResxItem.Key);
							leftFileChanged = true;
						}
						else
						{
							sourceResxItem.ReapplyDatesToComment();
							sourceResxItem.CopyToNode(ref sourceXmlDoc, ref node);
							leftFileChanged = true;
						}
					}
					if (destinationResxItem.Changed)
					{
						if (destinationResxItem.Deleted)
						{
							destinationResxItem.DeleteNode(ref destinationXmlDoc, destinationResxItem.Key);
							rightFileChanged = true;
						}
						else
						{
							destinationResxItem.ReapplyDatesToComment();
							destinationResxItem.CopyToNode(ref destinationXmlDoc, ref pairInDestinationXml);
							rightFileChanged = true;
						}
					}
					if (sourceResxItem.Changed || destinationResxItem.Changed)
					{
						customTraceLog.AddLine("");
					}
				}
			}
		}

		/// <summary>
		/// See Resolution matrix in ReadMe.txt!
		/// </summary>
		/// <param name="L"></param>
		/// <param name="R"></param>
		/// <param name="customTraceLog"></param>
		/// <returns></returns>
		private static bool SynchronizeResxItems(ref ResxItem L, ref ResxItem R, CustomTraceLog customTraceLog, DateTime now, out bool hasConflicts)
		{
			bool hasErrors = false;
			hasConflicts = false;

			//See Resolution matrix in ReadMe.txt!
			if(L==null && R!=null && R.Value!=null &&
			   R.LastSynchronizationDate==null)
			{
				L = new ResxItem();
				L.Key = R.Key;
				CopyToLeft(ref L, ref R, customTraceLog);
				AddDates(ref L, ref R, customTraceLog, now);
				RequireRebuild(customTraceLog);
				L.Changed = true;
				R.Changed = true;
				L.Created = true;
			}else
			if (L == null && R != null && R.Value != null &&
			R.LastSynchronizationDate != null)
			{
				customTraceLog.ExtendLastLine("Deleted right.");
				RequireRebuild(customTraceLog);
				L = new ResxItem();
				L.Key = R.Key;
				L.Deleted = true;
				R.Changed = true;
				L.Changed = true;
				R.Deleted = true;
			}else
			if (R == null && L != null && L.Value != null &&
			L.LastSynchronizationDate == null)
			{
				R = new ResxItem();
				R.Key = L.Key;
				CopyToRight(ref L, ref R, customTraceLog);
				AddDates(ref L, ref R, customTraceLog, now);
				RequireRebuild(customTraceLog);
				R.Changed = true;
				L.Changed = true;
				R.Created = true;
			}else
			if (R == null && L != null && L.Value != null &&
			    L.LastSynchronizationDate != null)
			{
				customTraceLog.ExtendLastLine("Deleted left.");
				RequireRebuild(customTraceLog);
				R = new ResxItem();
				R.Key = L.Key;
				R.Deleted = true;
				L.Changed = true;
				R.Changed = true;
				L.Deleted = true;
			}else
			if (L.LastSynchronizationDate != null && R.LastSynchronizationDate != null &&
			   L.LastSynchronizationDate != R.LastSynchronizationDate)
			{
				customTraceLog.ExtendLastLine("Error: Last sync dates does not match. Are you syncing right versions of resx folders?!");
				hasErrors = true;
				L.Changed = false;
				R.Changed = false;
			}else
			if (
				L.Value != null && R.Value != null && L.Value==R.Value &&
				(L.LastModificationDate==null || L.LastSynchronizationDate==null || R.LastModificationDate==null || R.LastSynchronizationDate==null
				|| (L.LastModificationDate!=L.LastSynchronizationDate || R.LastSynchronizationDate!=R.LastModificationDate))
				)
			{
				AddDates(ref L, ref R, customTraceLog, now);
				L.Changed = true;
				R.Changed = true;
			}
			else
			if (L.Value != null && L.LastSynchronizationDate == null && L.LastModificationDate == null &&
				R.Value == null && R.LastSynchronizationDate == null && R.LastModificationDate == null)
			{
				CopyToRight(ref L,ref R,customTraceLog);
				AddDates(ref L, ref R, customTraceLog, now);
				RequireRebuild(customTraceLog);
				L.Changed = true;
				R.Changed = true;
			}else
			if (L.Value == null && L.LastSynchronizationDate == null && L.LastModificationDate == null &&
				R.Value != null && R.LastSynchronizationDate == null && R.LastModificationDate == null)
			{
				CopyToLeft(ref L, ref R, customTraceLog);
				AddDates(ref L, ref R, customTraceLog, now);
				RequireRebuild(customTraceLog);
				L.Changed = true;
				R.Changed = true;
			}else
		    if (L.Value != null && L.LastSynchronizationDate != null && L.LastModificationDate != null &&
				R.Value != null && R.LastSynchronizationDate == null && R.LastModificationDate == null &&
				L.Value!=R.Value && L.LastSynchronizationDate==L.LastModificationDate)
			{
				CopyToLeft(ref L, ref R, customTraceLog);
				AddDates(ref L, ref R, customTraceLog, now);
				L.Changed = true;
				R.Changed = true;
			}else
			if (L.Value != null && L.LastSynchronizationDate == null && L.LastModificationDate == null &&
			R.Value != null && R.LastSynchronizationDate != null && R.LastModificationDate != null &&
			L.Value != R.Value && R.LastSynchronizationDate == R.LastModificationDate)
			{
				CopyToRight(ref L, ref R, customTraceLog);
				AddDates(ref L, ref R, customTraceLog, now);
				L.Changed = true;
				R.Changed = true;
			}else
			if (L.Value != null && L.LastSynchronizationDate != null && L.LastModificationDate != null &&
			R.Value != null && R.LastSynchronizationDate == null && R.LastModificationDate == null &&
			L.Value != R.Value && L.LastSynchronizationDate != L.LastModificationDate)
			{
				customTraceLog.ExtendLastLine("Conflict. None changed.");
				hasConflicts = true;
				L.Changed = false;
				R.Changed = false;
			}
			else
			if (L.Value != null && L.LastSynchronizationDate == null && L.LastModificationDate == null &&
			R.Value != null && R.LastSynchronizationDate != null && R.LastModificationDate != null &&
			L.Value != R.Value && R.LastSynchronizationDate != R.LastModificationDate)
			{
				customTraceLog.ExtendLastLine("Conflict. None changed.");
				hasConflicts = true;
				L.Changed = false;
				R.Changed = false;
			}
			else
		    if (L.Value != null && L.LastSynchronizationDate != null && L.LastModificationDate != null &&
			R.Value != null && R.LastSynchronizationDate != null && R.LastModificationDate != null &&
			L.Value == R.Value && L.LastModificationDate == R.LastModificationDate)
			{
				//customTraceLog.ExtendLastLine("None changed.");
				L.Changed = false;
				R.Changed = false;
			}
			else
			if (L.Value != null && L.LastSynchronizationDate != null && L.LastModificationDate != null &&
			R.Value != null && R.LastSynchronizationDate != null && R.LastModificationDate != null &&
			L.Value != R.Value && L.LastSynchronizationDate == L.LastModificationDate && R.LastSynchronizationDate == R.LastModificationDate)
			{
				customTraceLog.ExtendLastLine("Error: Sync and modifications dates are same but text is different.");
				hasErrors = true;
				L.Changed = false;
				R.Changed = false;
			}
			else
			if (L.Value != null && L.LastSynchronizationDate != null && L.LastModificationDate != null &&
			R.Value != null && R.LastSynchronizationDate != null && R.LastModificationDate != null &&
			L.Value != R.Value && L.LastSynchronizationDate != L.LastModificationDate && R.LastSynchronizationDate == R.LastModificationDate)
			{
				CopyToRight(ref L, ref R, customTraceLog);
				AddDates(ref L, ref R, customTraceLog, now);
				L.Changed = true;
				R.Changed = true;
			}
			else
			if (L.Value != null && L.LastSynchronizationDate != null && L.LastModificationDate != null &&
			R.Value != null && R.LastSynchronizationDate != null && R.LastModificationDate != null &&
			L.Value != R.Value && L.LastSynchronizationDate == L.LastModificationDate && R.LastSynchronizationDate != R.LastModificationDate)
			{
				CopyToLeft(ref L, ref R, customTraceLog);
				AddDates(ref L, ref R, customTraceLog, now);
				L.Changed = true;
				R.Changed = true;
			}
			else
			if (L.Value != null && L.LastSynchronizationDate != null && L.LastModificationDate != null &&
			R.Value != null && R.LastSynchronizationDate != null && R.LastModificationDate != null &&
			L.Value != R.Value && L.LastSynchronizationDate != L.LastModificationDate && R.LastSynchronizationDate != R.LastModificationDate)
			{
				customTraceLog.ExtendLastLine("Conflict. None changed.");
				hasConflicts = true;
				L.Changed = false;
				R.Changed = false;
			}else
			if (L.Value != null && L.LastSynchronizationDate == null && L.LastModificationDate == null &&
			R.Value != null && R.LastSynchronizationDate == null && R.LastModificationDate == null &&
			L.Value != R.Value)
			{
				customTraceLog.ExtendLastLine("Conflict. None changed.");
				hasConflicts = true;
				L.Changed = false;
				R.Changed = false;
			}
			else
			{
				customTraceLog.ExtendLastLine(string.Format("Error: Unsupported case happened.Send log to devel to exemine.\r\nLeft:\r\n{0}\r\nRight:\r\n{1}", L.ToString(),R.ToString()));
				hasErrors = true;
				L.Changed = false;
				R.Changed = false;
			}

			return hasErrors;
		}

		#region Resolution methods
		private static void CopyToRight(ref ResxItem L, ref ResxItem R, CustomTraceLog customTraceLog)
		{
			R.Value = L.Value;
			R.Comment = L.Comment;
			customTraceLog.ExtendLastLine("Copied to right.");
		}

		private static void CopyToLeft(ref ResxItem L, ref ResxItem R, CustomTraceLog customTraceLog)
		{
			L.Value = R.Value;
			L.Comment = R.Comment;
			customTraceLog.ExtendLastLine("Copied to Left.");
		}

		private static void AddDates(ref ResxItem L, ref ResxItem R, CustomTraceLog customTraceLog, DateTime now)
		{
			L.LastSynchronizationDate = L.LastModificationDate = R.LastSynchronizationDate = R.LastModificationDate = now;
			customTraceLog.ExtendLastLine("Dates updated.");
		}

		private static void RequireRebuild(CustomTraceLog customTraceLog)
		{
			customTraceLog.ExtendLastLine("Rebuild is required.");
		}
		#endregion   
	
	}
}
