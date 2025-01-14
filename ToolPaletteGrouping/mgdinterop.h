#pragma once

#include <vcclr.h>
#include <gcroot.h>
///////////////////////////////////////////////////////////////////////////////
// Forward Declarations
//

// Unmanaged types
class AcGeVector2d;
class AcGeVector3d;
class AcGeMatrix2d;
class AcGeMatrix3d;
class AcGePoint2d;
class AcGePoint3d;
class AcGeScale2d;
class AcGeScale3d;
class AcGeTol;
class AcDbObjectId;
class AcDbExtents;

// Managed types
namespace Autodesk
{
  namespace AutoCAD
  {
    namespace Runtime
    {
#ifdef __cplusplus_cli
      ref class DisposableWrapper;
#else
      public __gc __abstract class DisposableWrapper;
#endif
    }
  }
}

#ifdef __cplusplus_cli
#define AC_GCHANDLE_TO_VOIDPTR(x) ((GCHandle::operator System::IntPtr(x)).ToPointer())
#define AC_VOIDPTR_TO_GCHANDLE(x) (GCHandle::operator GCHandle(System::IntPtr(x)))
#define AC_NULLPTR nullptr
#define AC_GCNEW gcnew
#define AC_WCHAR_PINNED_GCPTR pin_ptr<const wchar_t>
typedef Autodesk::AutoCAD::Runtime::DisposableWrapper^ DisposableWrapper_GcPtr;
typedef System::Type^ Type_GcPtr;
typedef System::String^ String_GcPtr;
#else
#define AC_GCHANDLE_TO_VOIDPTR(x) ((GCHandle::op_Explicit(x)).ToPointer())
#define AC_VOIDPTR_TO_GCHANDLE(x) (GCHandle::op_Explicit(x))
#define AC_NULLPTR 0
#define AC_GCNEW new
#define AC_WCHAR_PINNED_GCPTR const wchar_t __pin*
typedef Autodesk::AutoCAD::Runtime::DisposableWrapper* DisposableWrapper_GcPtr;
typedef System::Type* Type_GcPtr;
typedef System::String* String_GcPtr;
#endif



///////////////////////////////////////////////////////////////////////////////
// Data Marshalling
//

class StringToWchar
{
  typedef System::Runtime::InteropServices::GCHandle GCHandle;

  const wchar_t* m_ptr;
  void* m_pinner;

public:

  StringToWchar(String_GcPtr str)
  {
    //pin the string
    m_pinner = AC_GCHANDLE_TO_VOIDPTR(
      GCHandle::Alloc(str, System::Runtime::InteropServices::GCHandleType::Pinned)
    );
    AC_WCHAR_PINNED_GCPTR tmp = PtrToStringChars(str);
    m_ptr = tmp;
  }
  ~StringToWchar()
  {
    GCHandle g = AC_VOIDPTR_TO_GCHANDLE(m_pinner);
    g.Free();
    m_pinner = 0;
  }
  operator const wchar_t* ()
  {
    return m_ptr;
  }
};
inline String_GcPtr WcharToString(const wchar_t* value)
{
  return AC_GCNEW System::String(value);
}

#undef AC_GCHANDLE_TO_VOIDPTR
#undef AC_VOIDPTR_TO_GCHANDLE
#undef AC_NULLPTR
#undef AC_GCNEW


//these defines make legacy clients happy
#define StringToCIF StringToWchar
#define CIFToString WcharToString
