
class Person():
	def __init__(self, _id, pos, avSpeed):
		self.id = _id
		self.pos = pos
		self.avSpeed = avSpeed
		self.actSpeed = avSpeed

class Customer(Person):
	def __init__(self, time):
		self.timeStore = time

class Cashier(Person):
	def __init__(self, inCash):
		self.inCash = inCash