$oldPartition = Get-Partition -DiskNumber 0 -PartitionNumber 4
if($oldPartition.Size -lt 100GB)
{
  write-host "Resize partition..."
  $size = (Get-PartitionSupportedSize -DiskNumber 0 -PartitionNumber 4)
  Resize-Partition -DiskNumber 0 -PartitionNumber 4 -Size $size.SizeMax
  $newPartition = Get-Partition -DiskNumber 0 -PartitionNumber 4
  write-host ($env:COMPUTERNAME + ":`t" + $newPartition.Size)
}
else
{
    write-host ($env:COMPUTERNAME + ":`t" + $oldPartition.Size)
}
