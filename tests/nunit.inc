<?xml version="1.0"?>
<project name="boo nunit include">

	<!-- find out where nunit.framework.dll is -->
	<property name="lib.dir"
		value="${path::combine(nant::get-base-directory(), 'lib')}"
		dynamic="true" />

    <property name="nunit.framework.dll"
			value="${path::combine(lib.dir, 'common/2.0/nunit.framework.dll')}" />

</project>
