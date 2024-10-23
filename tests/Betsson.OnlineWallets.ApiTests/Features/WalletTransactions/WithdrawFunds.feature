@withdraw
Feature: Withdraw Funds
  As a user or API client,
  I want to withdraw funds from my online wallet account
  So that I can use the funds for external payments or other needs.

  Background:
    Given the API endpoint for withdrawal funds is '/withdraw'
    And I check my current balance before transaction

   @HappyPath
  Scenario: Successfully withdraw funds when there is sufficient balance
    Given the user has a balance higher than '50.00'
    When the user withdraws '50.00'
    Then the transacction should be successful with status 'Completed' 
    And the response status code should be '200'
    And the status description should be 'OK' 
    And the balance should be updated to reflect the 'withdraw' transaction

  @ErrorResponse
  Scenario: Fail to withdraw funds due to insufficient balance
    When the user attempts to withdraw an amount greater than their current balance
    Then the response status should be 'Error'    
    And the response status code should be '400'
    And the status description should be 'Bad Request'
    And the exception type is 'InsufficientBalanceException'
    And the exception error message is 'Invalid withdrawal amount. There are insufficient funds.'
    And the balance should not be updated

  @EdgeCase
  Scenario: Successfully withdraw all funds from the account
    When the user withdraws all the funds from account
    Then the transacction should be successful with status 'Completed' 
    And the response status code should be '200'
    And the status description should be 'OK' 
    And the new balance should be '0.00'

  @EdgeCase
  Scenario: Fail to withdraw with invalid amount (negative)
    Given the user has a balance higher than '50.00'
    When the user withdraws '-50.00'
    Then the response status should be 'Error'
    And the response status code should be '400'
    And the status description should be 'Bad Request'    
    And the response should contain an error message ''Amount' must be greater than or equal to '0'.'
    And the balance should not be updated