Feature: ClientServer

A short summary of the feature


Scenario: Create Client and Server
	Given create Server at port 8888
	When connect to Server at port 8888
	Then connection result should be True

Scenario: Execute a remote method
	Given execute method Sum at Server side with parameters 1 and 2
	Then remote sum result should be 3