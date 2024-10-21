  Feature: Wallet Balance
  As an Api client
  I want to request the balance 
  So that I can retrieve the current balance

      

  Scenario Outline: Successfully retrieve a list of users for a specific page
    Given The user has an existing wallet
    When I request the balance
    Then the response should show the correct balance