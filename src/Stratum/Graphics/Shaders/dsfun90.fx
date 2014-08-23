// Emulation based on Fortran-90 double-single package. See http://crd.lbl.gov/~dhbailey/mpdist/
// Substract: res = ds_add(a, b) => res = a + b
float2 ds_add(float2 dsa, float2 dsb)
{
	float2 dsc;
	float t1, t2, e;

	t1 = dsa.x + dsb.x;
	e = t1 - dsa.x;
	t2 = ((dsb.x - e) + (dsa.x - (t1 - e))) + dsa.y + dsb.y;

	dsc.x = t1 + t2;
	dsc.y = t2 - (dsc.x - t1);
	return dsc;
}

// Substract: res = ds_sub(a, b) => res = a - b
float2 ds_sub(float2 dsa, float2 dsb)
{
	float2 dsc;
	float e, t1, t2;

	t1 = dsa.x - dsb.x;
	e = t1 - dsa.x;
	t2 = ((-dsb.x - e) + (dsa.x - (t1 - e))) + dsa.y - dsb.y;

	dsc.x = t1 + t2;
	dsc.y = t2 - (dsc.x - t1);
	return dsc;
}

// Compare: res = -1 if a < b
//              = 0 if a == b
//              = 1 if a > b
float ds_compare(float2 dsa, float2 dsb)
{
	if (dsa.x < dsb.x) return -1;
	else if (dsa.x == dsb.x)
	{
		if (dsa.y < dsb.y) return -1;
		else if (dsa.y == dsb.y) return 0;
		else return 1;
	}
	else return 1;
}

// Multiply: res = ds_mul(a, b) => res = a * b
float2 ds_mul(float2 dsa, float2 dsb)
{
	float2 dsc;
	float c11, c21, c2, e, t1, t2;
	float a1, a2, b1, b2, cona, conb, split = 8193;

	cona = dsa.x * split;
	conb = dsb.x * split;
	a1 = cona - (cona - dsa.x);
	b1 = conb - (conb - dsb.x);
	a2 = dsa.x - a1;
	b2 = dsb.x - b1;

	c11 = dsa.x * dsb.x;
	c21 = a2 * b2 + (a2 * b1 + (a1 * b2 + (a1 * b1 - c11)));

	c2 = dsa.x * dsb.y + dsa.y * dsb.x;

	t1 = c11 + c2;
	e = t1 - c11;
	t2 = dsa.y * dsb.y + ((c2 - e) + (c11 - (t1 - e))) + c21;

	dsc.x = t1 + t2;
	dsc.y = t2 - (dsc.x - t1);

	return dsc;
}

float2 ds_div(float2 a, float2 b)
{
	float2 c;
	float s1, cona, conb, a1, b1, a2, b2, c11, c21;
	float c2, t1, e, t2, t12, t22, t11, t21, s2;

	// Compute a DP approximation to the quotient.
	s1 = a.x / b.x;

	// This splits s1 and b.x into high-order and low-order words.
	cona = s1 * 8193.0f;
	conb = b.x * 8193.0f;
	a1 = cona - (cona - s1);
	b1 = conb - (conb - b.x);
	a2 = s1 - a1;
	b2 = b.x - b1;

	// Multiply s1 * dsb(1) using Dekker's method.
	c11 = s1 * b.x;
	c21 = (((a1 * b1 - c11) + a1 * b2) + a2 * b1) + a2 * b2;

	// Compute s1 * b.y (only high-order word is needed).
	c2 = s1 * b.y;

	// Compute (c11, c21) + c2 using Knuth's trick.
	t1 = c11 + c2;
	e = t1 - c11;
	t2 = ((c2 - e) + (c11 - (t1 - e))) + c21;

	// The result is t1 + t2, after normalization.
	t12 = t1 + t2;
	t22 = t2 - (t12 - t1);

	// Compute dsa - (t12, t22) using Knuth's trick.

	t11 = a.x - t12;
	e = t11 - a.x;
	t21 = ((-t12 - e) + (a.x - (t11 - e))) + a.y - t22;

	// Compute high-order word of (t11, t21) and divide by b.x.
	s2 = (t11 + t21) / b.x;

	// The result is s1 + s2, after normalization.
	c.x = s1 + s2;
	c.y = s2 - (c.x - s1);

	return c;
}

// create double-single number from float
float2 ds_set(float a)
{
	float2 z;
	z.x = a;
	z.y = 0.0;
	return z;
}