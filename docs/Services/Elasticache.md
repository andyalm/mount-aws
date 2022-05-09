# Elasticache on MountAws

TODO. In the meantime, you can just run `dir` within the `elasticache` directory and probably find your way around.
Remember that you can often `cd` into objects to find more related objects within. The hierarchy looks like this:

```
-- elasticache
   |-- clusters
       |-- my-cache-cluster-id
           |-- 0001 #Cache Node ID
           |-- 0002
   |-- replication-groups
       |-- my-replication-group-id
           |-- my-cache-cluster-001
               |-- 0001 #Cache Node ID
           |-- my-cache-cluster-002
               |-- 0002 #Cache Node ID
```