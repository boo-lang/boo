int i = 4;	// look: tinyc cannot do this, but we can!

int *i;



int f(char c, char *d)

{

	int f = 5;	// look: tinyc cannot do this, but we can!

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



