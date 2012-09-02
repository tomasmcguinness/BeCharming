//
//  Listener.m
//  BeCharming
//
//  Created by Tomas McGuinness on 02/09/2012.
//  Copyright (c) 2012 tomasmcguinness.com. All rights reserved.
//

#import "Listener.h"

@implementation Listener

- (void)start;
{
    self.socket = [[GCDAsyncUdpSocket alloc] initWithDelegate:self delegateQueue:dispatch_get_main_queue()];
    self.socket.delegate = self;
    
    NSError *error = nil;
    
    if(![self.socket bindToPort:22003 error:&error])
    {
        NSLog(@"I goofed: %@", error);
    }
    
    if (![self.socket beginReceiving:&error])
    {
        NSLog(@"Error receiving: %@", [error description]);
        return;
    }
    
    if (![self.socket joinMulticastGroup:@"230.0.0.1" error:&error])
    {
        NSLog(@"I goofed: %@", error);
    }
}

- (void)stop
{
    [self.socket close];
    self.socket = nil;
}

- (void)udpSocket:(GCDAsyncUdpSocket *)sock
   didReceiveData:(NSData *)data
      fromAddress:(NSData *)address
withFilterContext:(id)filterContext
{
    NSLog(@"Did Receive Data");
    NSString *msg = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    if (msg)
    {
        NSLog(@"Message: %@",msg);
        
        // Respond!
        NSString *response = @"MacBookPro|false|5|111111-1111-1111-11111111";
        NSData *data = [response dataUsingEncoding:NSUTF8StringEncoding];
        
        NSString *host = nil;
        uint16_t port = 0;
        [GCDAsyncUdpSocket getHost:&host port:&port fromAddress:address];
        
        NSLog(@"Message received from: %@:%hu", host, port);

        [sock sendData:data toAddress:address withTimeout:-1 tag:0];
        [sock close];
    }
    else
    {
        NSString *host = nil;
        uint16_t port = 0;
        [GCDAsyncUdpSocket getHost:&host port:&port fromAddress:address];
        
        NSLog(@"Unknown Message: %@:%hu", host, port);
    }
}

@end
