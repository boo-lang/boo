


// wh: bug(?) in DotGNU 0.6 - "using antlr" will workaround the problem.
#if __CSCC__
using antlr;
#endif

class CSCCBUG_11519 {}   


// The above will make the example to compilable by CSCC but that does
// not guarantee that we can execute. On Mandrake 1o and cscc 0.6.0 I'
// m getting this on execution:
//
//  $ ilrun ./test1.exe 
//  SupportTest::Main [2487] - callvirt at verify_call.c:2031
//  Uncaught exception: System.Security.VerificationException: Could not verify the code
//
// Giving up.