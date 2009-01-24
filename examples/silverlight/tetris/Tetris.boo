#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


#Silverlight Tetris Example
#Author: Vladimir Lazunin
#
#How to compile:
#booc -nostdlib -target:library -lib:"C:\Program Files\Microsoft SDKs\Silverlight\v2.0\Reference Assemblies","C:\Program Files\Microsoft SDKs\Silverlight\v2.0\Libraries\Client" Tetris.boo

namespace Tetris

import System
import System.Windows
import System.Windows.Controls
import System.Windows.Media
import System.Windows.Shapes
import System.Windows.Input
import System.Windows.Threading

######## Silverlight stuff ########
class MyPage(UserControl):

	canvas = Canvas()
	textblock = TextBlock()
	textblock2 = TextBlock()

	gColors = [Colors.Black, Colors.Gray, Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Orange, Colors.White, Colors.Magenta]

	leds as (Rectangle, 2)
	matr as (int, 2)

	tserv = TetServer()

	paused = false
	timer = DispatcherTimer()

	def constructor():
		#a block of text where we print how many lines have you cleared
		textblock.FontSize = 24
		textblock.Text = "Lines: 0"
		canvas.Children.Add(textblock)
		canvas.SetLeft(textblock, 100)
		canvas.SetTop(textblock, 500)

		#another one, where we print "pause" and "finish"
		textblock2.FontSize = 30
		textblock2.Text = ''
		textblock2.Foreground = SolidColorBrush(Colors.Yellow)

		matr = tserv.frame()

		leds = matrix(Rectangle, len(matr, 0), len(matr, 1))

		n_r = 20
		for i in range(len(matr, 0)):
			n_c = 50
			for j in range(len(matr, 1)):
				r = Rectangle()
				r.Width = 20
				r.Height = 20
				r.Fill = SolidColorBrush(gColors[matr[i, j]])
				canvas.Children.Add(r)
				canvas.SetLeft(r, n_c)
				canvas.SetTop(r, n_r)
				leds[i, j] = r
				n_c += 20
			n_r += 20

		canvas.Children.Add(textblock2)
		canvas.SetLeft(textblock2, 100)
		canvas.SetTop(textblock2, 200)

		KeyDown += onKeyDown

		timer.Interval = TimeSpan(0, 0, 0, 0, 500)
		timer.Tick += onTimer
		timer.Start()

		self.Content = canvas


	#send e key kommand to the server, receive a matrix,
	#do the drawing
	def onKeyDown(sender as object, e as KeyEventArgs):
		if e.Key == Key.P:
			paused = not paused
			if paused: textblock2.Text = "...PAUSED..."
			else: textblock2.Text = ""

		if paused: return

		if e.Key == Key.Left:
			tserv.move_left()
		elif e.Key == Key.Right:
			tserv.move_right()
		elif e.Key == Key.Up:
			tserv.rotate()
		elif e.Key == Key.Down:
			tserv.fall(3)
		doDraw()


	def onTimer(sender as object, e as EventArgs):
		if paused: return

		tserv.fall_auto()
		doDraw()


	def doDraw():
		matr = tserv.frame()
		for i in range(len(matr, 0)):
			for j in range(len(matr, 1)):
				leds[i, j].Fill = SolidColorBrush(gColors[matr[i, j]])

		textblock.Text = "Lines: " + tserv.cleared_lines().ToString()

		if tserv.full():
			canvas.KeyDown -= onKeyDown
			timer.Tick -= onTimer
			textblock2.Text = "...FINISH..."


class MyApp(Application):
	mp = MyPage()
	def constructor():
		Startup += onStartup

	def onStartup(sender, e):
		self.RootVisual = mp


######## Tetris stuff ########
def rotate_cc(m as (int, 2)):
""" Rotates matrix 90 degrees counter-clockwise """
	m_new as (int, 2) = matrix(int, len(m, 1), len(m, 0))

	i_n = -1
	for j in range(len(m, 1)-1, -1, -1):
		i_n += 1
		j_n = -1
		for i in range(len(m, 0)):
			j_n += 1
			m_new[i_n, j_n] = m[i, j]
	return m_new

def does_collide(m_parent as (int, 2), m_child as (int, 2), c_row as int, c_col as int):
	for i in range(len(m_child, 0)):
		for j in range(len(m_child, 1)):
			ni, nj = i+c_row, j+c_col
			if (0 <= ni) and (ni < len(m_parent, 0)) and (0 <= nj) and (nj < len(m_parent, 1)):
				if m_child[i, j] != 0 and m_parent[ni, nj] != 0: return true
	return false

def matr_combine(m_parent as (int, 2), m_child as (int, 2), c_row as int, c_col as int):
	m_new = matr_copy(m_parent)

	for i in range(len(m_child, 0)):
		for j in range(len(m_child, 1)):
			if m_child[i, j] > 0: m_new[i+c_row, j+c_col] = m_child[i, j]
	return m_new

def matr_copy(src as (int, 2)):
	dst = matrix(int, len(src, 0), len(src, 1))
	for i in range(len(src, 0)):
		for j in range(len(src, 1)):
			dst[i, j] = src[i, j]
	return dst


class Figure:
	public matr as (int, 2)
	public row as int
	public col as int
	glass as Glass
	public settled = false

	def constructor(m as (int, 2), g as Glass, r as int, c as int):
		self.glass = g //reference
		matr = matr_copy(m)
		row = r
		col = c

	def rotate():
		matr_new = rotate_cc(matr)
		if not does_collide(glass.matr, matr_new, row, col):
			self.matr = matr_new

	def drop():
		if settled: return
		if not does_collide(glass.matr, self.matr, row+1, col):
			row += 1
		else:
			settled = true
			glass.settle(self)

	def move_left():
		if not does_collide(glass.matr, self.matr, row, col-1):
			col -= 1

	def move_right():
		if not does_collide(glass.matr, self.matr, row, col+1):
			col += 1


class Glass:
	public matr as (int, 2)
	twall as int = 1
	padding as int = 1
	public full = false
	width as int
	height as int
	public cleared as int = 0

	def constructor(height as int, width as int):
		self.width = width
		self.height = height
		matr = matrix(int, padding+height+twall+padding, padding+twall+width+twall+padding)
		for i in range(padding, padding+height):
			matr[i, padding] = 1
			matr[i, padding + twall + width] = 1 //?

		for i in range(padding, padding+width+twall+1):
			matr[padding+height, i] = 1

	def is_row_full(n_row):
		for i in range(padding+twall, padding+twall+width):
			if matr[n_row, i] == 0: return false
		return true

	def is_row_empty(n_row):
		for i in range(padding+twall, padding+twall+width):
			if matr[n_row, i] != 0: return false
		return true

	def destroy_row(n_row):
		for i in range(n_row, padding, -1):
			for j in range(padding+twall, padding+twall+width):
				matr[i, j] = matr[i-1, j]

	def compact():
		for i in range(padding+height):
			if is_row_full(i):
				destroy_row(i)
				cleared += 1		if not(is_row_empty(padding)): full = true

	def settle(fig as Figure):
		matr = matr_combine(matr, fig.matr, fig.row, fig.col)
		compact()


class TetServer:
	public glass as Glass
	public figure as Figure
	falling_speed = 0.5
	falling_acc = 0.0
	rnd = Random()

	#don't know how to declare the type of this list, so use this 'hack'
	#to make type inference do the job...
	fmatrices = [matrix(int, 3, 4)]

	def full():
		return glass.full

	def constructor():
		glass = Glass(20, 10)

		###### creating matrices for the figures
		###### setting elements is rather tedious - are there literals for 2d arrays?
		// I figure
		i = matrix(int, 3, 4)
		for j in range(4): i[1, j] = 2

		fmatrices[0] = i

		// L figure
		i = matrix(int, 4, 5)
		i[1, 1] = 3
		i[1, 2] = 3
		i[1, 3] = 3
		i[2, 1] = 3
		fmatrices.Add(i)

		// J figure
		i = matrix(int, 4, 5)
		i[1, 1] = 4
		i[1, 2] = 4
		i[1, 3] = 4
		i[2, 3] = 4
		fmatrices.Add(i)
		// O figure
		i = matrix(int, 4, 4)
		i[1, 1] = 5
		i[1, 2] = 5
		i[2, 1] = 5
		i[2, 2] = 5
		fmatrices.Add(i)

		// S figure
		i = matrix(int, 4, 5)
		i[1, 2] = 6
		i[1, 3] = 6
		i[2, 1] = 6
		i[2, 2] = 6
		fmatrices.Add(i)

		// T figure
		i = matrix(int, 5, 5)
		i[1, 2] = 7
		i[2, 1] = 7
		i[2, 2] = 7
		i[2, 3] = 7
		fmatrices.Add(i)

		// Z figure
		i = matrix(int, 4, 5)
		i[1, 1] = 8
		i[1, 2] = 8
		i[2, 2] = 8
		i[2, 3] = 8
		fmatrices.Add(i)

		next_figure()


	def cleared_lines():
		return glass.cleared

	def frame():
		return matr_combine(glass.matr, figure.matr, figure.row, figure.col)

	def next_figure():
		i = rnd.Next(len(fmatrices))
		figure = Figure(fmatrices[i], glass, 0, 5)

	def fall_auto():
		if figure.settled:
			next_figure()
		falling_acc += falling_speed
		if falling_acc >= 1.0:
			falling_acc = 0.0
			figure.drop()

	def fall(amount as int):
		for i in range(amount):
			figure.drop()

	def move_left():
		figure.move_left()

	def move_right():
		figure.move_right()

	def rotate():
		figure.rotate()
