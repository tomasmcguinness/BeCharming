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
    [self openBroadcastListener];
    [self openServiceListener];
}

- (void)stop
{
    [self.socket close];
    self.socket = nil;
}

- (void)openBroadcastListener
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

- (void)openServiceListener
{
    self.serverSocket = [[GCDAsyncSocket alloc] initWithDelegate:self delegateQueue:dispatch_get_main_queue()];
    self.serverSocket.delegate = self;
    
    NSError *error = nil;
    uint16_t port = 22001;
    if (![self.serverSocket acceptOnPort:port error:&error])
    {
        NSLog(@"I goofed: %@", error);
    }
    
//    if(![self.serverSocket bindToPort:22001 error:&error])
//    {
//        NSLog(@"I goofed: %@", error);
//    }
//    
//    if (![self.serverSocket beginReceiving:&error])
//    {
//        NSLog(@"Error receiving: %@", [error description]);
//        return;
//    }
}

- (void)socket:(GCDAsyncSocket *)sender didAcceptNewSocket:(GCDAsyncSocket *)newSocket
{
    // The "sender" parameter is the listenSocket we created.
    // The "newSocket" is a new instance of GCDAsyncSocket.
    // It represents the accepted incoming client connection.
    
    // Do server stuff with newSocket...
    newSocket.delegate = self;
    //newSocket readDataToData:<#(NSData *)#> withTimeout:<#(NSTimeInterval)#> buffer:<#(NSMutableData *)#> bufferOffset:<#(NSUInteger)#> maxLength:<#(NSUInteger)#> tag:<#(long)#>
}

- (void)socket:(GCDAsyncSocket *)sock didReadData:(NSData *)data withTag:(long)tag
{
    NSString *str = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    NSLog(@"%@",str);
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
        NSString *host = nil;
        uint16_t port = 0;
        [GCDAsyncUdpSocket getHost:&host port:&port fromAddress:address];
        
        NSLog(@"Message received from: %@:%hu", host, port);
        
        if([msg isEqualToString:@"**BECHARMING DISCOVERY**"])
        {
            // Respond!
            NSString *response = @"MacBookPro|false|5|111111-1111-1111-11111111";
            NSData *data = [response dataUsingEncoding:NSUTF8StringEncoding];
            
            [sock sendData:data toAddress:address withTimeout:-1 tag:0];
        }
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
