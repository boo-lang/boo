// wh: bug(?) in DotGNU 0.6 - "using antlr" will workaround the problem.
#if __CSCC__
using antlr;
#else 
class Dummy {}   
#endif
