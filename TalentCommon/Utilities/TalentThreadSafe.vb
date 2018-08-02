Imports Microsoft.VisualBasic
Imports System.Threading
Imports System.Web

<Serializable()> _
Public Class TalentThreadSafe

    Public Structure MyStructure
        Public CacheKey As String
        Public CacheDateTime As DateTime
    End Structure


    Private Shared DateTimeFormatString As String = "yyyy/MM/dd hh:mm:ss.fff"
    Private Shared sleepTime As Integer = 100
    Private Shared loopMax As Integer = 40

    Private Shared CacheQueue As ArrayList

    Public Shared Function ItemIsInCache(ByVal cacheKey As String) As Boolean

        'Should only ever execute the first time the function is called
        If CacheQueue Is Nothing Then CacheQueue = New ArrayList

        ItemIsInCache = False
        Try
            If Utilities.IsCacheActive Then

                Dim myStruct As New MyStructure
                myStruct.CacheDateTime = CType(Now.ToString(DateTimeFormatString), DateTime)
                myStruct.CacheKey = cacheKey

                Dim loopCount As Integer = 1

                Do While loopCount <= loopMax
                    If Not HttpContext.Current.Cache.Item(cacheKey) Is Nothing Then
                        'The item is in Cache so we can continue as no db access will occur
                        Exit Do
                    Else
                        'If the item was not in cache and no one is attempting to 
                        'return it from the DB then release the loop
                        'Dim myTime As New DateTime(myStruct.CacheDateTime.Ticks)
                        If ThreadSafe(myStruct, cacheKey, loopCount) Then
                            Exit Do
                        Else
                            'Do not sleep when we have hit the limit.  We need to add this threads structure
                            'to the queue as soon as possible
                            If loopCount = loopMax Then
                                Exit Do
                            Else
                                'If someone is attempting to get the data then sleep
                                'to give them chance to return and add the item to cache
                                'so it is ready for the next loop
                                System.Threading.Thread.Sleep(sleepTime)
                                loopCount += 1
                            End If
                        End If
                    End If
                Loop

                'Go and get the data if we have hit our limit and it still isn't in cache
                If HttpContext.Current.Cache.Item(cacheKey) Is Nothing Then
                    ' Set the new date and time to the cache queue  
                    myStruct.CacheDateTime = CType(Now.ToString(DateTimeFormatString), DateTime)
                    CacheQueue.Add(myStruct)
                Else
                    ItemIsInCache = True
                End If
            End If
        Catch ex As Exception
            Talent.Common.Utilities.TalentCommonLog("ThreadSafe - Error", "Items In Cache", ex.Message)
        End Try

        Return ItemIsInCache

    End Function


    Public Shared Function ThreadSafe(ByRef myStruct As MyStructure, _
                                        ByVal cacheKey As String, _
                                        ByRef loopCount As Integer) As Boolean

        Try

            Dim QueueEntryExists As Boolean = False

            For Each struct As MyStructure In CacheQueue
                ' Do we have a entry for this cache key in the cache queue
                If struct.CacheKey = myStruct.CacheKey Then
                    QueueEntryExists = True
                    'Has another thread been added to the queue before this thread
                    If myStruct.CacheDateTime <= struct.CacheDateTime Then
                        'Start the wait again
                        myStruct.CacheDateTime = CType(Now.ToString(DateTimeFormatString), DateTime)
                        loopCount = 0
                        Exit For
                    End If
                End If
            Next

            'Is another thread going to the db for the data
            ThreadSafe = False
            If Not QueueEntryExists Then
                'No then we are going on our way the db - Yipee!!!
                ThreadSafe = True
            End If

        Catch ex As Exception
            ThreadSafe = False
            Talent.Common.Utilities.TalentCommonLog("ThreadSafe - Error", "ThreadSafe", ex.Message)
        End Try

        Return ThreadSafe

    End Function

    Public Shared Sub RemoveCacheQueueRecord(ByVal cacheKey As String)

        'We cannot remove from a list whilst we are looping through it
        'so we have to copy each item we wish to remove into a new
        'list and then remove from our master list whilst looping
        'through the remove list.

        Try


            Dim removeList As New ArrayList
            Dim CurrentDateTime As DateTime = CType(Now.ToString(DateTimeFormatString), DateTime)
            For Each struct As MyStructure In CacheQueue
                If cacheKey = struct.CacheKey Or _
                        DateDiff(DateInterval.Minute, CurrentDateTime, struct.CacheDateTime) > 1 Then
                    Dim myStruct As New MyStructure
                    myStruct.CacheKey = struct.CacheKey
                    myStruct.CacheDateTime = struct.CacheDateTime
                    removeList.Add(myStruct)
                End If
            Next

            For Each struct As MyStructure In removeList
                Try
                    If CacheQueue.IndexOf(struct) >= 0 Then
                        CacheQueue.Remove(struct)
                    End If
                Catch ex As Exception
                    Talent.Common.Utilities.TalentCommonLog("ThreadSafe - Error", "RemoveCacheQueueRecord2", ex.Message)
                End Try
            Next

        Catch ex As Exception
            Talent.Common.Utilities.TalentCommonLog("ThreadSafe - Error", "RemoveCacheQueueRecord", ex.Message)
        End Try

    End Sub

End Class



