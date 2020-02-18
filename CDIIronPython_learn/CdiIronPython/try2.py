value=-1

def function0():
    print "function0()"

def function1(val):
    print "function1():  %s", val

def function2(val1, val2):
    print "function2():  %s, %s", val1, val2

def ifn0():
    " ifn0 doc string. "
    value = 0

def ifn1(arg):
    value += arg 

def ifn_print():
    print value

class class1:
    " class1 docstring."
    def __init__(self):
        " __init__ docstring."
        pass
    def innerFunction0():
        " innerFunction0() docstring."
        print "innerFunction0()"
    def innerFunction1(val):
        print "innerFunction1():  %s", val

def getInstance():
    return class1
