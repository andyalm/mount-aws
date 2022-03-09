# IAM on MountAws

TODO. In the meantime, you can just run `dir` within the `iam` directory and probably find your way around.
Remember that you can often `cd` into objects to find more related objects within. The hierarchy looks like this:

```
-- iam
   |-- policies
       |-- my-policy # Policy name
   |-- roles
       |-- my-role # Role name
           |-- policies
               |-- my-embedded-policy #name of an embedded policy
               |-- my-attached-policy #name of an attached policy
           |-- statements
               |-- my-policy-statement-sid #sid of a statement within one of the policies associated with the role
```