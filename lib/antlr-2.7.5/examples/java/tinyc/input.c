int i;
int *i;

int f(char c, char *d)
{
	int f;
	c = '\033'+'\47'+'\''+'\\';
	d = " \" '\\' foo";
	i = c+3*f;
	if ( i ) {
		f = c;
	}
	else {
		f = 1;
	}
}

