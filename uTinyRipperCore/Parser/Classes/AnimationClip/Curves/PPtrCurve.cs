using System.Collections.Generic;
using uTinyRipper.Converters;
using uTinyRipper.SerializedFiles;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.AnimationClips
{
	public struct PPtrCurve : IAssetReadable, IYAMLExportable, IDependent
	{
		public PPtrCurve(PPtrCurve copy, IReadOnlyList<PPtrKeyframe> keyframes) :
			this(copy.Path, copy.Attribute, copy.ClassID, copy.Script, keyframes)
		{
		}

		public PPtrCurve(string path, string attribute, ClassIDType classID, PPtr<MonoScript> script)
		{
			Attribute = attribute;
			Path = path;
			ClassID = classID;
			Script = script;
			Curve = null;
		}

		public PPtrCurve(string path, string attribute, ClassIDType classID, PPtr<MonoScript> script, IReadOnlyList<PPtrKeyframe> keyframes) :
			this(path, attribute, classID, script)
		{
			Curve = new PPtrKeyframe[keyframes.Count];
			for (int i = 0; i < keyframes.Count; i++)
			{
				Curve[i] = keyframes[i];
			}
		}

		public static bool operator ==(PPtrCurve left, PPtrCurve right)
		{
			if (left.Attribute != right.Attribute)
			{
				return false;
			}
			if (left.Path != right.Path)
			{
				return false;
			}
			if (left.ClassID != right.ClassID)
			{
				return false;
			}
			if (left.Script != right.Script)
			{
				return false;
			}
			return true;
		}

		public static bool operator !=(PPtrCurve left, PPtrCurve right)
		{
			if (left.Attribute != right.Attribute)
			{
				return true;
			}
			if (left.Path != right.Path)
			{
				return true;
			}
			if (left.ClassID != right.ClassID)
			{
				return true;
			}
			if (left.Script != right.Script)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 2017.1 and greater
		/// </summary>
		private static bool IsAlign(Version version) => version.IsGreaterEqual(2017);

		public static void GenerateTypeTree(TypeTreeContext context, string name)
		{
			context.AddNode(nameof(PPtrCurve), name);
			context.BeginChildren();
			context.AddVector(CurveName, PPtrKeyframe.GenerateTypeTree);
			context.AddString(AttributeName);
			context.AddString(PathName);
			context.AddNode(TypeTreeUtils.TypeStarName, ClassIDName, sizeof(int));
			context.AddPPtr(nameof(MonoScript), ScriptName);
			context.EndChildren();
		}

		public void Read(AssetReader reader)
		{
			Curve = reader.ReadAssetArray<PPtrKeyframe>();
			if (IsAlign(reader.Version))
			{
				reader.AlignStream();
			}

			Attribute = reader.ReadString();
			Path = reader.ReadString();
			ClassID = (ClassIDType)reader.ReadInt32();
			Script.Read(reader);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(CurveName, Curve.ExportYAML(container));
			node.Add(AttributeName, Attribute);
			node.Add(PathName, Path);
			node.Add(ClassIDName, (int)ClassID);
			node.Add(ScriptName, Script.ExportYAML(container));
			return node;
		}

		public IEnumerable<PPtr<Object>> FetchDependencies(DependencyContext context)
		{
			foreach (PPtr<Object> asset in context.FetchDependencies(Curve, CurveName))
			{
				yield return asset;
			}
			yield return context.FetchDependency(Script, ScriptName);
		}

		public override bool Equals(object obj)
		{
			if (obj is PPtrCurve pptrCurve)
			{
				return this == pptrCurve;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hash = 113;
			unchecked
			{
				hash = hash + 457 * Attribute.GetHashCode();
				hash = hash * 433 + Path.GetHashCode();
				hash = hash * 223 + ClassID.GetHashCode();
				hash = hash * 911 + Script.GetHashCode();
			}
			return hash;
		}

		public PPtrKeyframe[] Curve { get; set; }
		public string Attribute { get; set; }
		public string Path { get; set; }
		public ClassIDType ClassID { get; set; }

		public const string CurveName = "curve";
		public const string AttributeName = "attribute";
		public const string PathName = "path";
		public const string ClassIDName = "classID";
		public const string ScriptName = "script";

		public PPtr<MonoScript> Script;
	}
}
