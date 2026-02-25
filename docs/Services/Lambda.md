# Lambda on MountAws

You can run `dir` within the `lambda` directory and find your way around.
Remember that you can often `cd` into objects to find more related objects within. The hierarchy looks like this:

```
-- lambda
   |-- functions
       |-- my-function # Function name
           |-- aliases
               |-- my-alias # Alias name
           |-- versions
               |-- 1 # Version number
           |-- event-source-mappings
               |-- 12345678-1234-1234-1234-123456789012 # UUID
   |-- layers
       |-- my-layer # Layer name
           |-- 1 # Layer version number
```
