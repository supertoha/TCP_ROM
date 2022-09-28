Feature: ClientServer

Ability to connect to remote or local server through port 8888.
Then execute remote method.


@connection
Scenario: Create Client and Server
	Given create Server at port 8888
	When connect to Server at port 8888
	Then connection result should be True

@execute
Scenario: Execute a remote method Sum
	Given execute method Sum at Server side with parameters 1 and 2
	Then remote sum result should be 3

@perfomance
Scenario: Performance test
	Given Set random input buffer size 100 bytes
	And Execute method Concatenate 10000 times
	Then Sped will more then 10000 executes per second
