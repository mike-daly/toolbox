"""Test documentation for the whole file at the top....""" 

def SampleStaticFunction(data):
    """Test sample function."""
    print ("SampleStaticFunction( ", data, " )")

class CdiClient:
    """Test implementation to prototype cdi client in Python."""
    i = 0

    def ProcessOneEvent(self, event, watermark):
        """ProcessOneEvent() -- do the work for one event, update watermark"""
        self.i += 1
        print event.dateTime, '**', (event.now - event.dateTime), '**', event.message, self.i
        watermarkProvider.UpdateWatermark(watermark)

    def NoOp(self, someText):
        """Test sample member function."""
        print someText, self.i
        self.i += 1

    def ProcessDelegate(self, someDelegate):
        """Test delegated function callback."""
        print "testing delegated callback"
        someDelegate()

    def ProcessDelegateWithVal(self, someDelegate):
        """Test delegated function callback."""
        print "testing delegated callback"
        someDelegate("hi mom")
