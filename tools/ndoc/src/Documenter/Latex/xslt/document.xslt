<!--

NDoc XML to LaTeX XSLT sheet.

Copyright (C) 2002 Thong Nguyen (tum_public@veridicus.com)

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

-->

<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:variable name="lowercase" select="'abcdefghijklmnopqrstuvwxyz'"/>
<xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>

<xsl:template name="translate-type">
	<xsl:param name="type"/>
	<xsl:choose>
		<xsl:when test="$type='System.Void'">void</xsl:when>
		<xsl:when test="$type='System.Boolean'">bool</xsl:when>
		<xsl:when test="$type='System.Object'">object</xsl:when>
		<xsl:when test="$type='System.Int32'">int</xsl:when>
		<xsl:when test="$type='System.String'">string</xsl:when>
		<xsl:when test="$type='System.Int64'">long</xsl:when>
		<xsl:when test="$type='System.Single'">float</xsl:when>
		<xsl:when test="$type='System.Double'">double</xsl:when>
		<xsl:when test="$type='System.Byte'">byte</xsl:when>
		<xsl:when test="$type='System.Char'">char</xsl:when>
		<xsl:when test="$type='System.Decimal'">decimal</xsl:when>
		<xsl:otherwise><xsl:value-of select="$type"/></xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="get-base-type">
	<xsl:variable name="type" select="@baseType"/>
	
	<xsl:choose>
		<xsl:when test="string-length($type)=0">
			<xsl:choose>			
			<xsl:when test="name() = 'interface'"></xsl:when>
			<xsl:when test="name() = 'struct'">ValueType</xsl:when>
			<xsl:otherwise>Object</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:otherwise><xsl:value-of select="$type"/></xsl:otherwise>
	</xsl:choose>	
</xsl:template>

<xsl:template name="get-return-type">
	<xsl:variable name="type" select="@returnType"/>	
	<xsl:call-template name="translate-type">
		<xsl:with-param name="type" select="$type"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="get-type">
	<xsl:variable name="type" select="@type"/>	
	<xsl:call-template name="translate-type">
		<xsl:with-param name="type" select="$type"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="get-access">	
	<xsl:variable name="access" select="translate(@access, $uppercase, $lowercase)"/>
	<xsl:choose>
		<xsl:when test="$access='family'">protected</xsl:when>
		<xsl:when test="$access='nestedpublic'">public</xsl:when>
		<xsl:otherwise><xsl:value-of select="$access"/></xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="output-member">
		<xsl:param name="label" />
		
		\vskip -1.9ex
		
		\membername{<xsl:value-of select="@name"/>}					
		{
			<!-- Method/Constructor signature -->
			
				\tt 
				<xsl:call-template name="get-access"/> 
				
				<xsl:choose>
				
				<xsl:when test="name() = 'constructor'">
					{\bf <xsl:value-of select="../@name"/>}(  )
				</xsl:when>
				
				<xsl:when test="name() = 'method'">
					\tt
					{\bf <xsl:call-template name="get-return-type"/>}
					{\bf <xsl:value-of select="@name"/>}(  )
				</xsl:when>
				
				<xsl:when test="name() = 'property'">
					{\bf <xsl:call-template name="get-type"/>}
					{\bf <xsl:value-of select="@name"/>}
					\{ 						
						<xsl:if test="@get = 'true'"> get; </xsl:if>
						<xsl:if test="@get = 'true' and @set = 'true'">\space</xsl:if>
						<xsl:if test="@set = 'true'"> set; </xsl:if>
					\}
				</xsl:when>
				
				<xsl:when test="name() = 'field'">					
					{\bf <xsl:call-template name="get-type"/>}
					{\bf <xsl:value-of select="@name"/>}
				</xsl:when>
				
				</xsl:choose>
				
			\label{<xsl:value-of select="$label"/>}			
		}
		
		%end signature

		<xsl:if test="count(documentation/summary) > 0">						
			<xsl:value-of select="documentation/summary"/>
		</xsl:if>
			
		<xsl:if test="count(documentation/remarks) > 0 or count(parameter) > 0 or count(documentation/example) > 0">
		\begin{itemize}
			\sld
			
			<xsl:if test="count(documentation/remarks) > 0">
			\item
			{	
				{\bf Usage}
				\sld				
				\begin{itemize}
					\isep
					\item
					{
						<xsl:value-of select="documentation/remarks"/>
					}
				\end{itemize}				
			}
			</xsl:if>
			
				
			<xsl:if test="count(parameter) > 0">
			\item
			{
				\sld
				{\bf Parameters}
				
				\sld
				\isep
								
				\begin{itemize}
					\sld\isep
					<xsl:for-each select="parameter">					
					\item
					{
					
						<xsl:variable name="paramname" select="@name"/>
						\sld
						{\tt <xsl:value-of select="@name"/>} - <xsl:value-of select="../documentation/param[@name=$paramname]"/>
					
					}
					</xsl:for-each>
				\end{itemize}
			}			
			</xsl:if>
			
			<xsl:if test="count(documentation/example) > 0">
			\item
			{
				\sld
				{\bf Example}
				
				\sld
				\isep
				{\tt
				<xsl:value-of select="documentation/example"/>
				}
			}
			</xsl:if>
			
		\end{itemize}
		</xsl:if>
		
		
					
</xsl:template>		

<xsl:template match="/">

\documentclass[11pt]{report}
\def\bl{\mbox{}\newline\mbox{}\newline{}}
\headheight       14pt
\usepackage{ifthen}
\newcommand{\hide}[2]{
\ifthenelse{\equal{#1}{inherited}}%
{}%
{}%
}
\newcommand{\entityintro}[3]{%
  \hbox to \hsize{%
    \vbox{%
      \hbox to .2in{}%
    }%
    {\bf #1}%
    \dotfill\pageref{#2}%
  }
  \makebox[\hsize]{%
    \parbox{.4in}{}%
    \parbox[l]{5in}{%
      \vspace{1mm}\it%
      #3%
      \vspace{1mm}%
    }%
  }%
}
\newcommand{\isep}[0]{%
\setlength{\itemsep}{-.4ex}
}
\newcommand{\sld}[0]{%
\setlength{\topsep}{0em}
\setlength{\partopsep}{0em}
\setlength{\parskip}{0em}
\setlength{\parsep}{-1em}
}
\newcommand{\headref}[3]{%
\ifthenelse{#1 = 1}{%
\addcontentsline{toc}{section}{\hspace{\qquad}\protect\numberline{}{#3}}%
}{}%
\ifthenelse{#1 = 2}{%
\addcontentsline{toc}{subsection}{\hspace{\qquad}\protect\numerline{}{#3}}%
}{}%
\ifthenelse{#1 = 3}{%
\addcontentsline{toc}{subsubsection}{\hspace{\qquad}\protect\numerline{}{#3}}%
}{}%
\label{#3}%
\makebox[\textwidth][l]{#2 #3}%
}%
\newcommand{\membername}[1]{{\it #1}\linebreak}
\newcommand{\divideents}[1]{\vskip -1em\indent\rule{2in}{.5mm}}
\newcommand{\refdefined}[1]{
\expandafter\ifx\csname r@#1\endcsname\relax
\relax\else
{$($ in \ref{#1}, page \pageref{#1}$)$}
\fi}
\newcommand{\startsection}[4]{
\gdef\classname{#2}
\subsection{\label{#3}{\bf {\sc #1} #2}}{
\rule[1em]{\hsize}{4pt}\vskip -1em
\vskip .1in 
#4
}%
}
\newcommand{\startsubsubsection}[2]{
\subsubsection{\sc #1}{%
\rule[1em]{\hsize}{2pt}%
#2}
}
\usepackage{color}
\date{\today}
\pagestyle{myheadings}
\addtocontents{toc}{\protect\def\protect\packagename{}}
\addtocontents{toc}{\protect\def\protect\classname{}}
\markboth{\protect\packagename -- \protect\classname}{\protect\packagename -- \protect\classname}
\oddsidemargin 0in
\evensidemargin 0in
% \topmargin -.8in
\chardef\bslash=`\\
\textheight 8.4in
\textwidth 5.5in
\begin{document}
\sloppy
\raggedright
\tableofcontents
\gdef\packagename{}
\gdef\classname{}
\newpage

<xsl:for-each select="ndoc/assembly/module/namespace">
<xsl:sort order="ascending" select="@name"/>
<xsl:variable name="namespaceindex" select="position()"/>

\def\packagename{<xsl:value-of select="@name"/>}
\chapter{\bf Namespace <xsl:value-of select="@name"/>}
{		
	\vskip -.25in
	
	\hbox to 
	\hsize
	{
		\it Namespace Contents\hfil Page
	}
	
	\rule{\hsize}{.7mm}
	\vskip .13in
	
	\hbox{\bf Interfaces}
		
	<xsl:for-each select="interface">
	<xsl:sort order="ascending" select="@name"/>
	\entityintro{<xsl:value-of select="@name"/>}{linter<xsl:value-of select="concat($namespaceindex, position())"/>}{<xsl:value-of select="documentation/summary"/>}
	</xsl:for-each>
		
	\vskip .1in
	\rule{\hsize}{.7mm}
	\vskip .1in
	
	
	\hbox{\bf Classes}
		
	<xsl:for-each select="class">
	<xsl:sort order="ascending" select="@name"/>
	\entityintro{<xsl:value-of select="@name"/>}{lclass<xsl:value-of select="concat($namespaceindex, position())"/>}{<xsl:value-of select="documentation/summary"/>}
	</xsl:for-each>
		
	\vskip .1in
	\rule{\hsize}{.7mm}
	\vskip .1in
		
	
	\newpage

	\section{Interfaces}
	{
		<xsl:for-each select="interface">
		<xsl:sort order="ascending" select="@name"/>
		<xsl:variable name="interfaceindex" select="position()"/>
		
		<xsl:if test="$interfaceindex > 1">
		\newpage
		</xsl:if>
		
		\startsection{Interface}{<xsl:value-of select="@name"/>}{linter<xsl:value-of select="concat($namespaceindex, position())"/>}
		{
			{\small <xsl:value-of select="documentation/summary"/>}
			{\small <xsl:value-of select="documentation/remarks"/>}
			\vskip .1in
			
			\startsubsubsection{Declaration}
			{
				\fbox
				{
					\vbox
					{
						\hbox
						{
							<xsl:call-template name="get-access"/> interface <xsl:value-of select="@name"/>
						}
						
						\noindent
												
						\hbox
						{
							\indent
							{\bf \space \space \space : } <xsl:call-template name="get-base-type"/>	
						}
					}
				}				
			}		
						
			<xsl:if test="count(property) > 0">
			\startsubsubsection{Properties}
			{
				\vskip -2em
				\begin{itemize}
					<xsl:for-each select="property">
					<xsl:sort order="ascending" select="@name"/>		
					<xsl:variable name="propertyindex" select="position()"/>
					\item
					{
						<xsl:call-template name="output-member">
						<xsl:with-param name="label" select="concat('liprop', 'L', $namespaceindex, 'L', $interfaceindex, 'L', $propertyindex)" />
						</xsl:call-template>
						\vskip 1.5em
					}
					</xsl:for-each>
				\end{itemize}
			}
			</xsl:if>			
			
			<xsl:if test="count(method) > 0">
			\startsubsubsection{Methods}
			{
				\vskip -2em
				\begin{itemize}
					<xsl:for-each select="method">
					<xsl:sort order="ascending" select="@name"/>
					<xsl:variable name="methodindex" select="position()"/>
					\item
					{
						<xsl:call-template name="output-member">
						<xsl:with-param name="label" select="concat('limthd', 'L', $namespaceindex, 'L', $interfaceindex, 'L', $methodindex)" />
						</xsl:call-template>
						\vskip 1.5em
					}
					</xsl:for-each>
				\end{itemize}
			}
			</xsl:if>
			
			\startsubsubsection{Extended Information}
			{
				\vskip -2em				
				\begin{itemize}
				\item
				{
					Assembly: {\tt <xsl:value-of select="../../../@name"/> }
				}
				\end{itemize}
			}
		}
		</xsl:for-each>
	}
	
	
	\section{Classes}
	{
		<xsl:for-each select="class">
		<xsl:sort order="ascending" select="@name"/>
		<xsl:variable name="classindex" select="position()"/>
		
		<xsl:if test="$classindex > 1">
		\newpage
		</xsl:if>
		
		\startsection{Class}{<xsl:value-of select="@name"/>}{lclass<xsl:value-of select="concat($namespaceindex, position())"/>}
		{
			{\small <xsl:value-of select="documentation/summary"/>}
			{\small <xsl:value-of select="documentation/remarks"/>}
			\vskip .1in
			
			\startsubsubsection{Declaration}
			{
				\fbox
				{
					\vbox
					{
						\hbox
						{
							<xsl:call-template name="get-access"/> class <xsl:value-of select="@name"/>
						}
						
						\noindent
												
						\hbox
						{
							\indent
							{\bf \space \space \space : } <xsl:call-template name="get-base-type"/>	
						}
					}
				}				
			}		
			
			<xsl:if test="count(field) > 0">
			\startsubsubsection{Fields}
			{
				\vskip -2em
				\begin{itemize}
					<xsl:for-each select="field">
					<xsl:sort order="ascending" select="@name"/>		
					<xsl:variable name="fieldindex" select="position()"/>
					\item
					{
						<xsl:call-template name="output-member">
						<xsl:with-param name="label" select="concat('lcfield', 'L', $namespaceindex, 'L', $classindex, 'L', $fieldindex)" />
						</xsl:call-template>
						\vskip 1.5em
					}
					</xsl:for-each>
				\end{itemize}
			}
			</xsl:if>
			
			<xsl:if test="count(property) > 0">
			\startsubsubsection{Properties}
			{
				\vskip -2em
				\begin{itemize}
					<xsl:for-each select="property">
					<xsl:sort order="ascending" select="@name"/>		
					<xsl:variable name="propertyindex" select="position()"/>
					\item
					{
						<xsl:call-template name="output-member">
						<xsl:with-param name="label" select="concat('lcprop', 'L', $namespaceindex, 'L', $classindex, 'L', $propertyindex)" />
						</xsl:call-template>
						\vskip 1.5em
					}
					</xsl:for-each>
				\end{itemize}
			}
			</xsl:if>			

			<xsl:if test="count(constructor) > 0">
			\startsubsubsection{Constructors}
			{
				\vskip -2em
				\begin{itemize}
				
					<xsl:for-each select="constructor">
					<xsl:variable name="constructorindex" select="position()"/>
					\item
					{
						<xsl:call-template name="output-member">
							<xsl:with-param name="label" select="concat('lcctor', 'L', $namespaceindex, 'L', $classindex, 'L', $constructorindex)" />
						</xsl:call-template>
						\vskip 1.5em
					}
					</xsl:for-each>
				\end{itemize}
			}
			</xsl:if>			

			<xsl:if test="count(method) > 0">
			\startsubsubsection{Methods}
			{
				\vskip -2em
				\begin{itemize}
					<xsl:for-each select="method">
					<xsl:sort order="ascending" select="@name"/>
					<xsl:variable name="methodindex" select="position()"/>					
					\item					
					{						
						<xsl:call-template name="output-member">
						<xsl:with-param name="label" select="concat('lcmthd', 'L', $namespaceindex, 'L', $classindex, 'L', $methodindex)" />
						</xsl:call-template>
						\vskip 1.5em
					}
					</xsl:for-each>
				\end{itemize}
			}
			</xsl:if>
			
			\startsubsubsection{Extended Information}
			{
				\vskip -2em				
				\begin{itemize}
				\item
				{
					Assembly: {\tt <xsl:value-of select="../../../@name"/> }
				}
				\end{itemize}
			}
		}
		</xsl:for-each>
	}
}

</xsl:for-each>

\end{document}
</xsl:template>
</xsl:transform>
