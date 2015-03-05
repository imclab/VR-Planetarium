using System;

namespace LMLineDrawing {
  public class InvalidDataException : Exception { 
    public InvalidDataException (string message) : base(message) {}
  }
}