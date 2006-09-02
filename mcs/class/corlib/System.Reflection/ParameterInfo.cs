// System.Reflection.ParameterInfo
//
// Sean MacIsaac (macisaac@ximian.com)
//
// (C) 2001 Ximian, Inc.
// Copyright (C) 2004-2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection
{
#if NET_2_0
	[ComVisible (true)]
	[ComDefaultInterfaceAttribute (typeof (_ParameterInfo))]
#endif
	[Serializable]
	[ClassInterfaceAttribute (ClassInterfaceType.None)]
	public class ParameterInfo : ICustomAttributeProvider, _ParameterInfo {

		protected Type ClassImpl;
		protected object DefaultValueImpl;
		protected MemberInfo MemberImpl;
		protected string NameImpl;
		protected int PositionImpl;
		protected ParameterAttributes AttrsImpl;
		private UnmanagedMarshal marshalAs;

		protected ParameterInfo () {
		}

		internal ParameterInfo (ParameterBuilder pb, Type type, MemberInfo member, int position) {
			this.ClassImpl = type;
			this.MemberImpl = member;
			if (pb != null) {
				this.NameImpl = pb.Name;
				this.PositionImpl = pb.Position - 1;	// ParameterInfo.Position is zero-based
				this.AttrsImpl = (ParameterAttributes) pb.Attributes;
			} else {
				this.NameImpl = "";
				this.PositionImpl = position - 1;
				this.AttrsImpl = ParameterAttributes.None;
			}
		}

		/* to build a ParameterInfo for the return type of a method */
		internal ParameterInfo (Type type, MemberInfo member, UnmanagedMarshal marshalAs) {
			this.ClassImpl = type;
			this.MemberImpl = member;
			this.NameImpl = "";
			this.PositionImpl = -1;	// since parameter positions are zero-based, return type pos is -1
			this.AttrsImpl = ParameterAttributes.Retval;
			this.marshalAs = marshalAs;
		}

		public override string ToString() {
			Type elementType = ClassImpl;
			while (elementType.HasElementType) {
					elementType = elementType.GetElementType();
			}
			bool useShort = elementType.IsPrimitive || ClassImpl == typeof(void)
				|| ClassImpl.Namespace == MemberImpl.DeclaringType.Namespace;
			string result = useShort
				? ClassImpl.Name
				: ClassImpl.FullName;
			// MS.NET seems to skip this check and produce an extra space for return types
			if (!IsRetval) {
				result += ' ';
				result += NameImpl;
			}
			return result;
		}

		public virtual Type ParameterType {
			get {return ClassImpl;}
		}
		public virtual ParameterAttributes Attributes {
			get {return AttrsImpl;}
		}
		public virtual object DefaultValue {
			get {return DefaultValueImpl;}
		}

		public bool IsIn {
			get {return (AttrsImpl & ParameterAttributes.In) != 0;}
		}

		public bool IsLcid {
			get {return (AttrsImpl & ParameterAttributes.Lcid) != 0;}
		}

		public bool IsOptional {
			get {return (AttrsImpl & ParameterAttributes.Optional) != 0;}
		}

		public bool IsOut {
			get {return (AttrsImpl & ParameterAttributes.Out) != 0;}
		}

		public bool IsRetval {
			get {return (AttrsImpl & ParameterAttributes.Retval) != 0;}
		}

		public virtual MemberInfo Member {
			get {return MemberImpl;}
		}

		public virtual string Name {
			get {return NameImpl;}
		}

		public virtual int Position {
			get {return PositionImpl;}
		}

#if NET_2_0 || BOOTSTRAP_NET_2_0
		public
#else
		internal
#endif
		virtual extern int MetadataToken {
			[MethodImplAttribute (MethodImplOptions.InternalCall)]
			get;
		}

		public virtual object[] GetCustomAttributes (bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes (this, inherit);
		}

		public virtual object[] GetCustomAttributes (Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes (this, attributeType, inherit);
		}

		public virtual bool IsDefined( Type attributeType, bool inherit) {
			return MonoCustomAttrs.IsDefined (this, attributeType, inherit);
		}

		internal object[] GetPseudoCustomAttributes () {
			int count = 0;

			if (IsIn)
				count ++;
			if (IsOut)
				count ++;
			if (IsOptional)
				count ++;
			if (marshalAs != null)
				count ++;

			if (count == 0)
				return null;
			object[] attrs = new object [count];
			count = 0;

			if (IsIn)
				attrs [count ++] = new InAttribute ();
			if (IsOptional)
				attrs [count ++] = new OptionalAttribute ();
			if (IsOut)
				attrs [count ++] = new OutAttribute ();

			if (marshalAs != null)
				attrs [count ++] = marshalAs.ToMarshalAsAttribute ();

			return attrs;
		}			

#if NET_2_0 || BOOTSTRAP_NET_2_0
		[Obsolete ("Use ParameterInfo.GetOptionalCustomModifiers().")]
		public virtual Type[] OptionalCustomModifiers {
			get {
				return GetOptionalCustomModifiers ();
			}
		}

		[Obsolete ("Use ParameterInfo.GetRequiredCustomModifiers().")]
		public virtual Type[] RequiredCustomModifiers {
			get {
				return GetRequiredCustomModifiers ();
			}
		}

		[MonoTODO]
		public virtual Type[] GetOptionalCustomModifiers () {
			throw new NotImplementedException ();
		}

		[MonoTODO]
		public virtual Type[] GetRequiredCustomModifiers () {
			throw new NotImplementedException ();
		}

		[MonoTODO]
		public virtual object RawDefaultValue {
			get {
				throw new NotImplementedException ();
			}
		}
#endif

#if NET_1_1
		void _ParameterInfo.GetIDsOfNames ([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException ();
		}

		void _ParameterInfo.GetTypeInfo (uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException ();
		}

		void _ParameterInfo.GetTypeInfoCount (out uint pcTInfo)
		{
			throw new NotImplementedException ();
		}

		void _ParameterInfo.Invoke (uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams,
			IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException ();
		}
#endif
	}
}
